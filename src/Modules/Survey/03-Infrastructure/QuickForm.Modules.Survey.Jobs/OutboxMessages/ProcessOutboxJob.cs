using Dapper;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Quartz;
using QuickForm.Common.Application;
using QuickForm.Common.Domain;
using QuickForm.Common.Infrastructure;
using System.Data;
using System.Data.Common;
using QuickForm.Modules.Survey.Options;
using QuickForm.Modules.Survey.Persistence;
using QuickForm.Common.Domain.Method;

namespace QuickForm.Modules.Survey.Jobs;

[DisallowConcurrentExecution]
internal sealed class ProcessOutboxJob(
        IDbConnectionFactory dbConnectionFactory,
        IServiceScopeFactory serviceScopeFactory,
        IDateTimeProvider _dateTimeProvider,
        IOptions<OutboxOptions> _outboxOptions,
        ILogger<ProcessOutboxJob> _logger
    ) : IJob
{
    public async Task Execute(IJobExecutionContext context)
    {
        _logger.LogInformation("{Module} Beginning to process outbox messages", Schemas.Survey);
        await using DbConnection connection = await dbConnectionFactory.OpenConnectionAsync();
        await using DbTransaction transaction = await connection.BeginTransactionAsync();
        
        IReadOnlyList<OutboxMessageResponse> outboxMessages = await GetOutboxMessagesAsync(connection, transaction);
        
        foreach (OutboxMessageResponse outboxMessage in outboxMessages)
        {
            List<ResultError> listResultError = new List<ResultError>();

            try
            {
                IDomainEvent domainEvent = JsonPrototype.Deserialize<IDomainEvent>(outboxMessage.Content)!;
                
                using IServiceScope scope = serviceScopeFactory.CreateScope();

                IEnumerable<IDomainEventHandler> domainEventHandlers = DomainEventHandlersFactory.GetHandlers(
                    domainEvent.GetType(),
                    scope.ServiceProvider,
                    Application.AssemblyReference.Assembly);

                foreach (IDomainEventHandler domainEventHandler in domainEventHandlers)
                {
                    await domainEventHandler.Handle(domainEvent);
                }
            }
            catch (Exception caughtException)
            {
                _logger.LogError(
                    caughtException,
                    "{Module} - Exception while processing outbox message {MessageId}",
                    Schemas.Survey,
                    outboxMessage.Id);

                listResultError = CommonMethods.ConvertExceptionToResult(caughtException, "OutBox");
            }

            await UpdateOutboxMessageAsync(connection, transaction, outboxMessage, listResultError);
        }

        await transaction.CommitAsync();

        _logger.LogInformation("{Module} - Completed processing outbox messages", Schemas.Survey);
    }

    private async Task<IReadOnlyList<OutboxMessageResponse>> GetOutboxMessagesAsync(
          IDbConnection connection,
          IDbTransaction transaction)
    {
        string sql = $"""                                    
                      SELECT TOP ({_outboxOptions.Value.BatchSize}) 
                        id, 
                        content
                      FROM [{Schemas.Survey}].[outbox_messages] WITH (ROWLOCK, UPDLOCK)
                      WHERE Status = 0
                      ORDER BY OccurredOnUtc;
                      """;

        IEnumerable<OutboxMessageResponse> outboxMessages =
            await connection.QueryAsync<OutboxMessageResponse>(
            sql,
            transaction: transaction);
        var messagesList = outboxMessages.ToList();



        return messagesList;
    }
    
    private async Task UpdateOutboxMessageAsync(
                IDbConnection connection,
                IDbTransaction transaction,
                OutboxMessageResponse outboxMessage,
                List<ResultError> listResultError)
    {
        string? exceptionDetails = listResultError.Any() ? JsonPrototype.Serialize(listResultError, SerializerSettings.CleanInstance) : null;

        const string sql = $"""
                                UPDATE {Schemas.Survey}.[outbox_messages]
                                SET 
                                    Status = @Status,
                                    ProcessedOnUtc = @ProcessedOnUtc,
                                    Error = @Error
                                WHERE id = @Id
                             """;

        await connection.ExecuteAsync(
            sql,
            new
            {
                outboxMessage.Id,
                ProcessedOnUtc = _dateTimeProvider.UtcNow,
                Error = exceptionDetails,
                Status = listResultError.Any() ? OutboxStatus.Failed : OutboxStatus.Processed
            },
            transaction: transaction
        );
    }

    internal sealed record OutboxMessageResponse(Guid Id, string Content);
}
