using Dapper;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Quartz;
using QuickForm.Common.Application;
using QuickForm.Common.Infrastructure;
using QuickForm.Modules.Users.Options;
using QuickForm.Modules.Users.Persistence;
using System.Data.Common;
using System.Data;

namespace QuickForm.Modules.Users.Jobs;
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
        logger.LogInformation("{Module} - Beginning to process inbox messages", Schemas.Auth);

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
                    Schemas.Auth,
                    inboxMessage.Id);

                exception = caughtException;
            }

            await UpdateInboxMessageAsync(connection, transaction, inboxMessage, exception);
        }

        await transaction.CommitAsync();

        logger.LogInformation("{Module} - Completed processing inbox messages", Schemas.Auth);
    }

    private async Task<IReadOnlyList<InboxMessageResponse>> GetInboxMessagesAsync(
        IDbConnection connection,
        IDbTransaction transaction)
    {
        string sql = $"""                                    
                      SELECT TOP ({inboxOptions.Value.BatchSize}) 
                        id, 
                        content
                      FROM [{Schemas.Auth}].[inbox_messages] WITH (ROWLOCK, UPDLOCK)
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
                                UPDATE {Schemas.Auth}.[inbox_messages]
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
