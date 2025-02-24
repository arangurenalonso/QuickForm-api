using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Quartz;
using QuickForm.Common.Application;
using QuickForm.Common.Infrastructure;
using System.Data.Common;
using System.Data;
using QuickForm.Modules.Survey.Options;
using QuickForm.Modules.Survey.Persistence;
using Dapper;
namespace QuickForm.Modules.Survey.Jobs;
[DisallowConcurrentExecution]
internal sealed class ProcessInboxJob(
    IDbConnectionFactory dbConnectionFactory,
    IServiceScopeFactory serviceScopeFactory,
    IDateTimeProvider dateTimeProvider,
    IOptions<InboxOptions> inboxOptions,
    ILogger<ProcessInboxJob> logger
    ) : IJob
{

    public async Task Execute(IJobExecutionContext context)
    {
        logger.LogInformation("{Module} - Beginning to process inbox messages", Schemas.Survey);

        await using DbConnection connection = await dbConnectionFactory.OpenConnectionAsync();
        await using DbTransaction transaction = await connection.BeginTransactionAsync();

        IReadOnlyList<InboxMessageResponse> inboxMessages = await GetInboxMessagesAsync(connection, transaction);

        foreach (InboxMessageResponse inboxMessage in inboxMessages)
        {
            Exception? exception = null;

            try
            {
                IIntegrationEvent integrationEvent = JsonPrototype.Deserialize<IIntegrationEvent>(inboxMessage.Content)!;

                using IServiceScope scope = serviceScopeFactory.CreateScope();

                IEnumerable<IIntegrationEventHandler> handlers = IntegrationEventHandlersFactory.GetHandlers(
                    integrationEvent.GetType(),
                    scope.ServiceProvider,
                    Presentation.AssemblyReference.Assembly);

                foreach (IIntegrationEventHandler integrationEventHandler in handlers)
                {
                    await integrationEventHandler.Handle(integrationEvent, context.CancellationToken);
                }
            }
            catch (Exception caughtException)
            {
                logger.LogError(
                    caughtException,
                    "{Module} - Exception while processing inbox message {MessageId}",
                    Schemas.Survey,
                    inboxMessage.Id);

                exception = caughtException;
            }

            await UpdateInboxMessageAsync(connection, transaction, inboxMessage, exception);
        }

        await transaction.CommitAsync();

        logger.LogInformation("{Module} - Completed processing inbox messages", Schemas.Survey);
    }

    private async Task<IReadOnlyList<InboxMessageResponse>> GetInboxMessagesAsync(
        IDbConnection connection,
        IDbTransaction transaction)
    {
        string sql = $"""                                    
                      SELECT TOP ({inboxOptions.Value.BatchSize}) 
                        id, 
                        content
                      FROM [{Schemas.Survey}].[inbox_messages] WITH (ROWLOCK, UPDLOCK)
                      WHERE Status = 0
                      ORDER BY OccurredOnUtc;
                      """;

        IEnumerable<InboxMessageResponse> inboxMessages = 
            await connection.QueryAsync<InboxMessageResponse>(
            sql,
            transaction: transaction);
        var messageList = inboxMessages.ToList();
        return messageList;
    }

    private async Task UpdateInboxMessageAsync(
        IDbConnection connection,
        IDbTransaction transaction,
        InboxMessageResponse inboxMessage,
        Exception? exception)
    {

        string? exceptionDetails = exception != null ? JsonPrototype.Serialize(exception) : null;

        const string sql = $"""
                                UPDATE {Schemas.Survey}.[inbox_messages]
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
                inboxMessage.Id,
                ProcessedOnUtc = dateTimeProvider.UtcNow,
                Error = exceptionDetails,
                Status = exception == null ? OutboxStatus.Processed : OutboxStatus.Failed
            },
            transaction: transaction);
    }

    internal sealed record InboxMessageResponse(Guid Id, string Content);
}
