using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using Dapper.Linq.Helpers;

namespace Dapper.SQLite
{
    public  class BulkInsertSQLiteProvider :BulkInsertProvider
    {
        private readonly SQLiteDbContext _db;

        public BulkInsertSQLiteProvider(SQLiteDbContext db) : base(db)
        {
            _db = db;
        }
    }
}
