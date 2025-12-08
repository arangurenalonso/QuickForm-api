using QuickForm.Common.Domain;

namespace QuickForm.Modules.Survey.Domain;
public sealed class FormActionDomain : BaseMasterEntity
{

    #region One-to-Many Relationship
    public ICollection<FormStatusPermissionDomain> Permissions { get; private set; } = [];
    #endregion
    public FormActionDomain() { }
    private FormActionDomain(MasterId id) : base(id) { }

    public static ResultT<FormActionDomain> Create(
            string keyName,
            string? description = null
        )
    {
        var newId = MasterId.Create();
        return Create(newId, keyName, description);
    }
    public static ResultT<FormActionDomain> Create(MasterId id, string keyName, string? description = null)
    {
        var newDomain = new FormActionDomain(id);
        var masterUpdateBase = new MasterUpdateBase(keyName, description);
        var result = newDomain.SetBaseProperties(masterUpdateBase);
        if (result.IsFailure)
        {
            return result.Errors;
        }
        return newDomain;
    }
}
