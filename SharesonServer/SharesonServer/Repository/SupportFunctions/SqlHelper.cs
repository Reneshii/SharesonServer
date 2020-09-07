using SharesonServer.Model.Support;
using System.Collections.Generic;
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
                var exists = DBExists("Shareson");
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
                    if(AreAllTablesExists() == false)
                    {
                        //DropTable("__MigrationHistory");
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
        /// Returns false if table is missing
        /// </summary>
        public bool AreAllTablesExists()
        {
            bool AddMissingTablesOnLatestMigrationVersion = true;
            string[] sqlTablesNames = System.Enum.GetNames(typeof(Enum.SqlTablesNames));

            var tablesInDB = DBContext.Database.SqlQuery<string>("SELECT name FROM Shareson.sys.tables").ToArray();

            foreach (var tablesDeclaredInCode in sqlTablesNames)
            {
                foreach (var tablesInSql in tablesInDB)
                {
                    if (/*tablesInDB.Equals(item)*/!string.Equals(tablesInSql, tablesDeclaredInCode, System.StringComparison.InvariantCultureIgnoreCase))
                    {
                        AddMissingTablesOnLatestMigrationVersion = false;
                    }
                }
            }
            return AddMissingTablesOnLatestMigrationVersion;
        }
        /// <summary>
        /// Returns true if db with given name exists
        /// </summary>
        public bool DBExists(string dbToCheck)
        {
            string CheckDB = $@"select name from master.dbo.sysdatabases where name = '"+ dbToCheck +"'";
            List<string> avaiableDBs = new List<string>();
            try
            {
                avaiableDBs = DBContext.Database.SqlQuery<string>(CheckDB).ToList();
            }
            catch { }

            if (avaiableDBs.Contains(dbToCheck))
            {
                avaiableDBs = null;
                return true;
            }
            else
            {
                avaiableDBs = null;
                return false;
            }
        }
        private void DropTable(string tableName)
        {
            string Clear = @"DROP TABLE " + tableName;
            DBContext.Database.ExecuteSqlCommand(Clear);
        }
        /// <summary>
        /// Returns account ID
        /// </summary>
        public string LogInToUserAccountAndGetID(string Email, string Password)
        {
            var acc = DBContext.Accounts.Where(f => f.Email == Email && f.Password == Password).FirstOrDefault();
            if(acc!= null)
            {
                acc.LoggedIn = true;
                DBContext.Accounts.Attach(acc);
                DBContext.SaveChanges();

                string accountID = DBContext.Accounts.Where(f => f.Email == Email && f.Password == Password).FirstOrDefault().ID;
                return accountID;
            }
            else
            {
                return "";
            }

        }

        public void LogOutUser(string Email, string Password)
        {
            var acc = DBContext.Accounts.Where(f => f.Email == Email && f.Password == Password).FirstOrDefault();
            acc.LoggedIn = false;
            DBContext.Accounts.Attach(acc);
            DBContext.SaveChanges();
        }
       
        public bool CreateAccount(AccountModel accountModel/*string Email, string Login, string Password, string Name, string Surname*/)
        {
            if(!IsAccountExist(accountModel.Email, accountModel.Password))
            {
                System.Guid guid = System.Guid.NewGuid();
                DBContext.Accounts.Add(new Model.Support.AccountModel
                {
                    ID = guid.ToString(),
                    Email = accountModel.Email, 
                    Login = accountModel.Login,
                    Password = accountModel.Password,
                    Name = accountModel.Name, 
                    Surname = accountModel.Surname,
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
        public string CreateAccount(AccountModel accountModel, bool preciseExceptionCase)
        {
            if (!IsAccountExist(accountModel.Email, accountModel.Password) && preciseExceptionCase == true)
            {
                System.Guid guid = System.Guid.NewGuid();
                DBContext.Accounts.Add(new Model.Support.AccountModel
                {
                    ID = guid.ToString(),
                    Email = accountModel.Email,
                    Login = accountModel.Login,
                    Password = accountModel.Password,
                    Name = accountModel.Name,
                    Surname = accountModel.Surname,
                    LoggedIn = false
                });
                DBContext.SaveChanges();
                return "Account Created";
            }
            else if(IsAccountExist(accountModel.Email, accountModel.Password) && preciseExceptionCase == true)
            {
                return "Account Already Exists";
            }
            else
            {
                return "Account Creation Faild";
            }
        }

        public bool IsAccountExist(string Email, string Password)
        {
            var emailResult = DBContext.Accounts.Where(f => f.Email == Email).FirstOrDefault();
            if (emailResult != null)
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

        public List<AccountModel> GetAccountsInfo()
        {
            return DBContext.Accounts.Where(f => f.ID != null).ToList();
        }
    }
}
