namespace QuickForm.Common.Infrastructure.Authorization;
public interface IPermissionService
{
    Task<bool> HasPermissionAsync(Guid userId, string resourceDescription, string actionDescription);
}
