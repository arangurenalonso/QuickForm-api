using System.Data.Common;

namespace QuickForm.Modules.Users.Persistence;
public interface IDbConnectionFactory
{
    ValueTask<DbConnection> OpenConnectionAsync();
}
