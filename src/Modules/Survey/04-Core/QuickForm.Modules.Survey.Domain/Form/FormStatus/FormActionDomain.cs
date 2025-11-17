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
        var newDomain = new FormActionDomain();
        var masterUpdateBase = new MasterUpdateBase(keyName, description);
        newDomain.SetBaseProperties(masterUpdateBase);

        return newDomain;
    }
    public static ResultT<FormActionDomain> Create(MasterId id, string keyName, string? description = null)
    {
        var newDomain = new FormActionDomain(id);
        var masterUpdateBase = new MasterUpdateBase(keyName, description);
        newDomain.SetBaseProperties(masterUpdateBase);

        return newDomain;
    }
}
