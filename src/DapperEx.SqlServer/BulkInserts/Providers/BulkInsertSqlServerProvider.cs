using DapperEx.BulkInserts.Providers;

namespace DapperEx.SqlServer.BulkInserts.Providers
{
    public class BulkInsertSqlServerProvider : BulkInsertProvider
    {

        public BulkInsertSqlServerProvider(SqlServerDbContext db) : base(db)
        {

        }
    }
}
