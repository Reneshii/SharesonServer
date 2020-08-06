
using System.Windows.Input;

namespace SharesonServer.Model.MainMenu
{
    public class MainMenuModel
    {
        public ICommand _startServer { get; set; }
        public ICommand _shutDownServer { get; set; }
        public object _MainMenuViewControlContent { get; set; }
        public int _ConnectedClients { get; set; }
        public bool _CanTurnOnServer { get; set; }
        public bool _CanTurnOffServer { get; set; }
    }
}
