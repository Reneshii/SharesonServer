using SharesonServer.Model.Support;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

namespace SharesonServer.Model.ForViews
{
    public class StartMenuModel
    {
        public ICommand _startServer { get; set; }
        public ICommand _shutDownServer { get; set; }

        public object _StartMenuControlContent { get; set; }
        public int _ConnectedClients { get; set; }
        public bool _CanTurnOnServer { get; set; }
        public bool _CanTurnOffServer { get; set; }
        public bool _ServerWithoutSQL { get; set; }
        public Visibility _CounterVisability { get; set; }

        public ObservableCollection<FullClientInfoModel> _UsersSource;
    }
}
