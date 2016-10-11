using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Dapper.SQLite
{
    public class BulkInsertSQLiteProvider : BulkInsertProvider
    {

        public BulkInsertSQLiteProvider(SQLiteDbContext db) : base(db)
        {

        }
    }
}
