﻿using System.Data.Entity;
using System.Data.Entity.Core.Common;
using System.Data.SQLite;
using System.Data.SQLite.EF6;

namespace CRY.UserSQLManager
{
    public class SQLiteConfiguration : DbConfiguration
    {
        public SQLiteConfiguration()
        {
            this.SetProviderFactory("System.Data.SQLite", SQLiteFactory.Instance);
            this.SetProviderFactory("System.Data.SQLite.EF6", SQLiteProviderFactory.Instance);
            this.SetProviderServices("System.Data.SQLite", (DbProviderServices)SQLiteProviderFactory.Instance.GetService(typeof(DbProviderServices)));
        }
    }
}
