using System.Windows.Input;

namespace SharesonServer.Model.MainMenu
{
    public class MainMenuModel
    {
        public ICommand _MenuStartBtn { get; set; }
        public ICommand _MenuAccountsSettingsBtn { get; set; }
        public ICommand _MenuServersSettingsBtn { get; set; }

        public object _MainMenuViewControlContent { get; set; }
    }
}
