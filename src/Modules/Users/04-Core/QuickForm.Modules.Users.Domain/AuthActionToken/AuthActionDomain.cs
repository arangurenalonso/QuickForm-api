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
        var newId = MasterId.Create();
        return Create(newId, keyName, description);
    }
    public static ResultT<AuthActionDomain> Create(MasterId id, string keyName, string? description = null)
    {
        var newDomain = new AuthActionDomain(id);
        var masterUpdateBase = new MasterUpdateBase(keyName, description);
        var result = newDomain.SetBaseProperties(masterUpdateBase);
        if (result.IsFailure)
        {
            return result.Errors;
        }

        return newDomain;
    }



}
