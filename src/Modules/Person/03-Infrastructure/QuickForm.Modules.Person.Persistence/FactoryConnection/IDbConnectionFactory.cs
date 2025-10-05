using System.Data.Common;

namespace QuickForm.Modules.Person.Persistence;
public interface IDbConnectionFactory
{
    ValueTask<DbConnection> OpenConnectionAsync();
}
