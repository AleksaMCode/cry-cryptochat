using System;
using System.Collections.Generic;
using System.Text;
using System.Data.Entity;
using System.Data.SQLite;

namespace CRY.UserSQLManager
{
    internal class UsersContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public UsersContext(string source) :
            base(
                new SQLiteConnection()
                {
                    ConnectionString = new SQLiteConnectionStringBuilder()
                    {
                        DataSource = source
                    }
                    .ConnectionString
                },
                true)
        {
            DbConfiguration.SetConfiguration(new SQLiteConfiguration());
        }
    }
}
