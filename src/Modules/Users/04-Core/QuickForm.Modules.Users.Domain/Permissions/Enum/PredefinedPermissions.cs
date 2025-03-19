using System.Reflection;
using QuickForm.Common.Domain;

namespace QuickForm.Modules.Users.Domain;
public static class PredefinedPermissions
{
    public static PredefinedPermission Auth_ChangePassword => 
        new PredefinedPermission(Guid.Parse("60FB6B55-DA96-4FFD-9755-B08E11E245B7"), ResourcesType.Auth, PermissionsActionType.ChangePassword);


    public static List<PredefinedPermission> GetAllPredefinedPermissions()
    {
        return typeof(PredefinedPermissions)
            .GetProperties(BindingFlags.Public | BindingFlags.Static)
            .Where(p => p.PropertyType == typeof(PredefinedPermission))
            .Select(p => (PredefinedPermission)p.GetValue(null)!)
            .ToList();
    }
}

public class PredefinedPermission
{
    public Guid Id { get; init; } 
    public ResourcesType ResourcesType { get; set; }
    public PermissionsActionType PermissionsActionType { get; set; }
    public PredefinedPermission(Guid id,ResourcesType resourcesType, PermissionsActionType permissionsActionType)
    {
        Id = id;
        ResourcesType = resourcesType;
        PermissionsActionType = permissionsActionType;
    }
    public override string ToString()
    {
        return $"{ResourcesType.GetDetail()}:{PermissionsActionType.GetDetail()}";
    }
}
