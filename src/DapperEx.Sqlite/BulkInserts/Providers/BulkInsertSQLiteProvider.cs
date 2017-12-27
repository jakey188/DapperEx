using DapperEx.BulkInserts.Providers;

namespace DapperEx.Sqlite.BulkInserts.Providers
{
    public class BulkInsertSqliteProvider : BulkInsertProvider
    {

        public BulkInsertSqliteProvider(SqliteDbContext db) : base(db)
        {

        }
    }
}
