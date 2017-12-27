using DapperEx.BulkInserts.Providers;

namespace DapperEx.MySql.BulkInserts.Providers
{
    public class BulkInsertmmMySqlProvider : BulkInsertProvider
    {

        public BulkInsertmmMySqlProvider(MySqlDbContext db) : base(db)
        {

        }
    }
}
