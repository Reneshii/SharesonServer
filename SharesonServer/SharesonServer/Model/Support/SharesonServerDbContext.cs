using SharesonServer.Model.Support.SQL;
using System.Data.Entity;

namespace SharesonServer.Model.Support
{
    public class SharesonServerDbContext : DbContext
    {
        public DbSet<AccountModel> Accounts { get; set; }
        public DbSet<DirectoriesAccessedInAccount> Directories { get; set; }

        public SharesonServerDbContext()
        {
            
        }
    }
}
