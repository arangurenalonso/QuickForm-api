namespace QuickForm.Common.Domain;

public class AuditLog 
{
    public Guid Id { get; private set; }
    public string? TableName { get; private set; }
    public Guid IdEntity { get; private set; }
    public AuditOperacionType? Action { get; private set; }
    public string? ActionName{ get; private set; }
    public string? OriginalValue { get; private set; }
    public string? CurrentValue { get; private set; }
    public string? ChangesValue { get; private set; }
    public Guid TransactionId { get; private set; }
    public DateTime? CreatedDate { get; private set; }
    public string UserTransaction { get; private set; }
    public string ClassOrigin { get; private set; }
    private AuditLog()
    {
    }
    private AuditLog(
        Guid id, 
        Guid idEntity,
        DateTime? createdDate, 
        string? tableName,
        AuditOperacionType? action, 
        string? actionName, 
        string? originalValue, 
        string? currentValue,
        string? changesValue,
        Guid transactionId,
        string userTransaction,
        string originClass
        ) 
    {
        Id = id;
        CreatedDate = createdDate;
        TableName = tableName;
        Action = action;
        ActionName = actionName;
        OriginalValue = originalValue;
        CurrentValue = currentValue;
        IdEntity = idEntity;
        ChangesValue = changesValue;
        TransactionId = transactionId;
        UserTransaction = userTransaction;
        ClassOrigin = originClass;

    }

    public static AuditLog Create( 
                                   Guid idEntity,
                                   DateTime? createdDate,
                                   string? tableName,
                                   AuditOperacionType action,
                                   string? actionName,
                                   string? originalValue,
                                   string? currentValue,
                                   string? changesValue,
                                   Guid transactionId,
                                   string userTransaction,
                                   string originClass)
    {
        var id = Guid.NewGuid();
        var auditLog = new AuditLog(id,
                                    idEntity,
                                    createdDate,
                                    tableName,
                                    action,
                                    actionName,
                                    action == AuditOperacionType.Added?null: originalValue,
                                    currentValue,
                                    action == AuditOperacionType.Added ? null : changesValue,
                                    transactionId,
                                    userTransaction,
                                    originClass);
        return auditLog;
    }
}
