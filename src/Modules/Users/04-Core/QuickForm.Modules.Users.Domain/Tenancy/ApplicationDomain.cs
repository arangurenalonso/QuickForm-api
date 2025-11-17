using QuickForm.Common.Domain;
namespace QuickForm.Modules.Users.Domain;
public sealed class ApplicationDomain : BaseMasterEntity
{

    #region One-to-Many Relationship
    public ICollection<PermissionsDomain> Permissions { get; private set; } = [];
    #endregion

    public ApplicationDomain() { }
    private ApplicationDomain(MasterId id) : base(id) { }

    public static ResultT<ApplicationDomain> Create(
            string keyName,
            string? description = null
        )
    {
        var newDomain = new ApplicationDomain();
        var masterUpdateBase = new MasterUpdateBase(keyName, description);
        newDomain.SetBaseProperties(masterUpdateBase);

        return newDomain;
    }
    public static ResultT<ApplicationDomain> Create(MasterId id, string keyName, string? description = null)
    {
        var newDomain = new ApplicationDomain(id);
        var masterUpdateBase = new MasterUpdateBase(keyName, description);
        newDomain.SetBaseProperties(masterUpdateBase);

        return newDomain;
    }


}
