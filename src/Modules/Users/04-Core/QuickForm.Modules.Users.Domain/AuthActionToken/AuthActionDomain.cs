using QuickForm.Common.Domain;

namespace QuickForm.Modules.Users.Domain;
public class AuthActionDomain : BaseMasterEntity 
{
    #region Many-to-Many Relationship
    public ICollection<AuthActionTokenDomain> UserActionTokens { get; private set; } = [];

    #endregion

    private AuthActionDomain() { }
    private AuthActionDomain(MasterId id) : base(id) { }

    public static ResultT<AuthActionDomain> Create(
            string keyName,
            string? description = null
        )
    {
        var newDomain = new AuthActionDomain();
        var masterUpdateBase = new MasterUpdateBase(keyName, description);
        newDomain.SetBaseProperties(masterUpdateBase);

        return newDomain;
    }
    public static ResultT<AuthActionDomain> Create(MasterId id, string keyName, string? description = null)
    {
        var newDomain = new AuthActionDomain(id);
        var masterUpdateBase = new MasterUpdateBase(keyName, description);
        newDomain.SetBaseProperties(masterUpdateBase);

        return newDomain;
    }



}
