using Dapper;
using QuickForm.Common.Application;
using QuickForm.Common.Infrastructure;
using QuickForm.Modules.Survey.Persistence;
using System.Data.Common;

namespace QuickForm.Modules.Survey.Host;
internal sealed class IdempotentIntegrationEventHandler<TIntegrationEvent>(
    IIntegrationEventHandler<TIntegrationEvent> decorated,
    IDbConnectionFactory dbConnectionFactory)
    : IntegrationEventHandler<TIntegrationEvent>
    where TIntegrationEvent : IIntegrationEvent
{
    public override async Task Handle(
        TIntegrationEvent integrationEvent,
        CancellationToken cancellationToken = default)
    {
        await using DbConnection connection = await dbConnectionFactory.OpenConnectionAsync();

        var inboxMessageConsumer = new InboxMessageConsumer(integrationEvent.Id, decorated.GetType().Name);

        if (await InboxConsumerExistsAsync(connection, inboxMessageConsumer))
        {
            return;
        }

        await decorated.Handle(integrationEvent, cancellationToken);

        await InsertInboxConsumerAsync(connection, inboxMessageConsumer);
    }

    private static async Task<bool> InboxConsumerExistsAsync(
        DbConnection dbConnection,
        InboxMessageConsumer inboxMessageConsumer)
    {
        const string sql =
          $"""
            SELECT CASE 
                WHEN EXISTS (
                    SELECT 1
                    FROM {Schemas.Survey}.[inbox_message_consumers]
                    WHERE [InboxMessageId] = @InboxMessageId 
                      AND [Name] = @Name
                ) THEN 1 ELSE 0 END
            """;

        return await dbConnection.ExecuteScalarAsync<bool>(sql, inboxMessageConsumer);
    }

    private static async Task InsertInboxConsumerAsync(
        DbConnection dbConnection,
        InboxMessageConsumer inboxMessageConsumer)
    {
        const string sql =
            $"""
            INSERT INTO {Schemas.Survey}.inbox_message_consumers(InboxMessageId, Name)
            VALUES (@InboxMessageId, @Name)
            """;

        await dbConnection.ExecuteAsync(sql, inboxMessageConsumer);
    }
}
