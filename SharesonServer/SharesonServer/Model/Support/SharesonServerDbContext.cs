using System.Data.Entity;

namespace SharesonServer.Model.Support
{
    public class SharesonServerDbContext : DbContext
    {
        public DbSet<AccountModel> Accounts { get; set; }
        public SharesonServerDbContext()
        {

        }
    }
}
