using SharesonServer.Model.Support;
using System.Linq;
using System.Threading.Tasks;

namespace SharesonServer.Repository.SupportFunctions
{
    public class SqlHelper
    {
        SharesonServerDbContext DBContext;

        public SqlHelper()
        {
            DBContext = new SharesonServerDbContext();
        }

        public Task<bool> StartSQL()
        {
            Task<bool> SQL_TASK = Task<bool>.Factory.StartNew(() =>
            {
                var exists = Exists("Shareson");
                if (exists == false)
                {
                    var DBCreated = CreateDBBasedOnLatestMigrationVersionIfNotExists();
                    if(DBCreated == true)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    if(CompareCompatibilityOfTables() == true)
                    {
                        //DropTable("MigrationHistory");
                        CreateDBBasedOnLatestMigrationVersionIfNotExists();
                    }
                    return true;
                }

            });
            return SQL_TASK;
        }

        /// <summary>
        /// Create new db or adds missing tables. IMPORTANT = Delete migration history in DB if table is missing.
        /// </summary>
        /// <returns></returns>
        public bool CreateDBBasedOnLatestMigrationVersionIfNotExists()
        {
            try
            {
                System.Data.Entity.Database.SetInitializer(new System.Data.Entity.MigrateDatabaseToLatestVersion<SharesonServerDbContext, Migrations.Configuration>());
                DBContext.Database.Initialize(false);
                return true;
            }
            catch
            {
                return false;
            }
        }
        /// <summary>
        /// Returns true if table is missing
        /// </summary>
        public bool CompareCompatibilityOfTables()
        {
            bool AddMissingTablesOnLatestMigrationVersion = false;
            string[] tablesInCode = { "Account" };
            var tablesInDB = DBContext.Database.SqlQuery<string>("SELECT name FROM Shareson.sys.tables").ToArray();

            foreach (var item in tablesInCode)
            {
                if (!tablesInDB.Contains(item))
                {
                    AddMissingTablesOnLatestMigrationVersion = true;
                }
            }
            return AddMissingTablesOnLatestMigrationVersion;
        }
        /// <summary>
        /// Returns true if db with given name exists
        /// </summary>
        public bool Exists(string dbToCheck)
        {
            string CheckDB =$@"select name from master.dbo.sysdatabases where name = '"+ dbToCheck +"'";

            var result = DBContext.Database.SqlQuery<string>(CheckDB).ToList();
            if(result[0].Contains(dbToCheck))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        //private void DropTable(string tableName)
        //{
        //    string Clear = @"DROP TABLE " + tableName ;
        //    DBContext.Database.ExecuteSqlCommand(Clear);
        //}
        /// <summary>
        /// Returns account ID
        /// </summary>
        public string LogInToUserAccount(string Email, string Password)
        {
            var acc = DBContext.Accounts.Where(f => f.Email == Email && f.Password == Password).FirstOrDefault();
            acc.LoggedIn = true;
            DBContext.Accounts.Attach(acc);
            DBContext.SaveChanges();

            string accountID = DBContext.Accounts.Where(f => f.Email == Email && f.Password == Password).FirstOrDefault().ID;
            return accountID;
        }
        public void LogOutUser(string Email, string Password)
        {
            var acc = DBContext.Accounts.Where(f => f.Email == Email && f.Password == Password).FirstOrDefault();
            acc.LoggedIn = false;
            DBContext.Accounts.Attach(acc);
            DBContext.SaveChanges();
        }
       
        public bool CreateAccount(string Email, string Login, string Password, string Name, string Surname)
        {
            if(!IsAccountExist(Email,Password))
            {
                System.Guid guid = System.Guid.NewGuid();
                DBContext.Accounts.Add(new Model.Support.AccountModel
                {
                    ID = guid.ToString(),
                    Email = Email, 
                    Login = Login,
                    Password = Password,
                    Name = Name, 
                    Surname = Surname,
                    LoggedIn = false
                });
                DBContext.SaveChanges();
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool IsAccountExist(string Email, string Password)
        {
            var emailResult = DBContext.Accounts.Select(f => f.Email == Email).FirstOrDefault();
            if (emailResult)
            {
                var passResult = DBContext.Accounts.Select(f => f.Password == Password).FirstOrDefault();
                if (passResult)
                {
                    return true;

                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
    }
}
