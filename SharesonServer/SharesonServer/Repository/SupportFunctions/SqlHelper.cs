using SharesonServer.Model.Support;
using SharesonServer.Model.Support.SQL;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SharesonServer.Repository.SupportFunctions
{
    public class SqlHelper
    {
        public static bool SQLStatus;
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
                        SQLStatus = true;
                        return SQLStatus;
                    }
                    else
                    {
                        SQLStatus = false;
                        return SQLStatus;
                    }
                }
                else
                {
                    if(AreAllTablesExists() == false)
                    {
                        //DropTable("__MigrationHistory");
                        CreateDBBasedOnLatestMigrationVersionIfNotExists();
                    }
                    SQLStatus = true;
                    return SQLStatus;
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
        public void LogInToUserAccount(string Email, string Password)
        {
            var acc = DBContext.Accounts.Where(f => f.Email == Email && f.Password == Password)?.FirstOrDefault();
            if(acc!= null)
            {
                acc.LoggedIn = true;
                DBContext.Accounts.Attach(acc);
                DBContext.SaveChanges();

                string accountID = DBContext.Accounts.Where(f => f.Email == Email && f.Password == Password)?.FirstOrDefault().ID;
                
            }
        }
        public bool CheckIfUserIsLogedIn(string ID, string Email)
        {
            AccountModel model;
            if(!string.IsNullOrEmpty(ID) && !string.IsNullOrEmpty(Email))
            {
                model = DBContext.Accounts.Where(f => f.ID == ID && f.Email == Email).FirstOrDefault();
                return model.LoggedIn;
            }
            else
            {
                return false;
            }
        }
        public AccountModelForShareson GetUserInfo(string Email, string Password)
        {
            DirectoriesAccessedInAccount sqlAcc = new DirectoriesAccessedInAccount();
            var acc = DBContext.Accounts.Where(f => f.Email == Email && f.Password == Password).FirstOrDefault();
            if(acc != null)
            {
                sqlAcc = DBContext.Directories.Where(f => f.ID == acc.ID).FirstOrDefault();
            }

            AccountModelForShareson model = new AccountModelForShareson();
            if (acc != null)
            {
                model = new AccountModelForShareson()
                {
                    ID = acc.ID != null ? acc.ID : string.Empty,
                    Name = acc.Name != null ? acc.Name : string.Empty,
                    Email = acc.Email != null ? acc.Email : string.Empty,
                    Login = acc.Login != null ? acc.Login : string.Empty,
                    Surname = acc.Surname != null ? acc.Surname : string.Empty,
                    Password = acc.Password != null ? acc.Password : string.Empty,
                };
            }
            else
            {
                model = new AccountModelForShareson();
            }
            List<string> splitedPaths = new List<string>();
            if (sqlAcc.Directories != null && sqlAcc.Directories.Length > 0)
            {
                var Directories = sqlAcc.Directories.Split(',');
                for (int i = 0; i < Directories.Length; i++)
                {
                    splitedPaths.Add(Directories[i]);
                }
                model.AccessedDirectory = splitedPaths.ToArray();
            }

            splitedPaths.Clear();
            return model;
        }

        public void LogOutUser(string Email, string Password)
        {
            var acc = DBContext.Accounts.Where(f => f.Email == Email && f.Password == Password).FirstOrDefault();
            acc.LoggedIn = false;
            DBContext.Accounts.Attach(acc);
            DBContext.SaveChanges();
        }
       
        public bool CreateAccount(AccountModelForShareson accountModel)
        {
            if(!IsAccountExist(accountModel.Email, accountModel.Password))
            {
                System.Guid guid = System.Guid.NewGuid();
                DBContext.Accounts.Add(new AccountModel
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
        public string CreateAccount(AccountModelForShareson accountModel, bool preciseExceptionCase)
        {
            if (!IsAccountExist(accountModel.Email, accountModel.Password) && preciseExceptionCase == true)
            {
                System.Guid guid = System.Guid.NewGuid();
                DBContext.Accounts.Add(new AccountModel
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

        //public List<AccountModel> GetUsersInfo()
        //{
        //    var SQLAccModel = DBContext.Accounts.Where(f => f.ID != null).ToList();



        //    return 
        //}
    }
}
