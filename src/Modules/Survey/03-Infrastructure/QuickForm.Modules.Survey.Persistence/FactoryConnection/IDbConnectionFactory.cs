using System.Data.Common;

namespace QuickForm.Modules.Survey.Persistence;
public interface IDbConnectionFactory
{
    ValueTask<DbConnection> OpenConnectionAsync();
}
