﻿using Dapper;
using MassTransit;
using QuickForm.Common.Application;
using QuickForm.Common.Infrastructure;
using QuickForm.Modules.Users.Persistence;
using System.Data.Common;

namespace QuickForm.Modules.Users.Host;
internal sealed class IntegrationEventConsumer<TIntegrationEvent>(IDbConnectionFactory dbConnectionFactory)
    : IConsumer<TIntegrationEvent>
    where TIntegrationEvent : IntegrationEvent
{
    public async Task Consume(ConsumeContext<TIntegrationEvent> context)
    {
        await using DbConnection connection = await dbConnectionFactory.OpenConnectionAsync();

        TIntegrationEvent integrationEvent = context.Message;

        var inboxMessage = new InboxMessage
        {
            Id = integrationEvent.Id,
            Type = integrationEvent.GetType().Name,
            Content = JsonPrototype.Serialize(integrationEvent),
            OccurredOnUtc = integrationEvent.OccurredOnUtc,
            IdOutboxMessage = integrationEvent.IdOutboxMessage,
        };

        const string sql =
            $"""
            INSERT INTO {Schemas.Auth}.[inbox_messages]
                (id, type, content, OccurredOnUtc,Status,IdOutboxMessage)
            VALUES 
                (@Id, @Type, @Content, @OccurredOnUtc,0,@IdOutboxMessage)
            """;

        await connection.ExecuteAsync(sql, inboxMessage);
    }
}
