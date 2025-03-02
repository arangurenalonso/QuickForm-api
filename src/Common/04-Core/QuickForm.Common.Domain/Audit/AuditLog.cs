namespace QuickForm.Common.Domain;

public class AuditLog 
{
    public Guid Id { get; private set; }
    public Guid IdEntity { get; private set; }

    public DateTime? CreatedDate { get; private set; }

    public string? TableName { get; private set; }

    public AuditOperacionType? Type { get; private set; }

    public string? TypeName { get; private set; }

    public string? OriginalValue { get; private set; }

    public string? CurrentValue { get; private set; }
    public string? ChangesValue { get; private set; }
    public Guid TransactionId { get; private set; }
    public string UserTransaction { get; private set; }

    private AuditLog()
    {
    }
    private AuditLog(
        Guid id, 
        Guid idEntity,
        DateTime? createdDate, 
        string? tableName,
        AuditOperacionType? type, 
        string? typeName, 
        string? originalValue, 
        string? currentValue,
        string? changesValue,
        Guid transactionId,
        string userTransaction
        ) 
    {
        Id = id;
        CreatedDate = createdDate;
        TableName = tableName;
        Type = type;
        TypeName = typeName;
        OriginalValue = originalValue;
        CurrentValue = currentValue;
        IdEntity = idEntity;
        ChangesValue = changesValue;
        TransactionId = transactionId;
        UserTransaction = userTransaction;

    }

    public static AuditLog Create( Guid id,
                                   Guid idEntity,
                                   DateTime? createdDate,
                                   string? tableName,
                                   AuditOperacionType type,
                                   string? typeName,
                                   string? originalValue,
                                   string? currentValue,
                                   string? changesValue,
                                   Guid transactionId,
                                   string userTransaction)
    {
        var auditLog = new AuditLog(id,
                                    idEntity,
                                    createdDate,
                                    tableName,
                                    type,
                                    typeName,
                                    type == AuditOperacionType.Added?null: originalValue,
                                    currentValue,
                                    type == AuditOperacionType.Added ? null : changesValue,
                                    transactionId,
                                    userTransaction);
        return auditLog;
    }
}
