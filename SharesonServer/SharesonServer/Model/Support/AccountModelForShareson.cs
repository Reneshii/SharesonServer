
namespace SharesonServer.Model.Support.SQL
{
    public class AccountModelForShareson
    {
        public readonly string Email;
        public readonly string ID;
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public string[] AccessedDirectory{ get; set; }
        public string Message { get; set; }

        public AccountModelForShareson(string email = null, string id = null)
        {
            Email = email;
            ID = id;
        }
    }
}
