namespace SharesonServer.Model.Support
{
    public class FullClientInfoModel
    {
        public string Email { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public string ID { get; set; }
        public bool LoggedIn { get; set; }
        public float ConnectionTime { get; set; }
        public string IP { get; set; }
    }
}
