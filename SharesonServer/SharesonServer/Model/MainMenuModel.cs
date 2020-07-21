
using System.Windows.Input;

namespace SharesonServer.Model.MainMenu
{
    public class MainMenuModel
    {
        public ICommand _startServer { get; set; }
        public ICommand _shutDownServer { get; set; }
        public bool _isServerWorks { get; set; }
        public object _MainMenuViewControlContent { get; set; }
    }
}
