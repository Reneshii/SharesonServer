using Shareson.Support;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace SharesonServer.Model.ForControls
{
    public class ServersSettingsModel
    {
        public ICommand _SaveSettings;
        public ICommand _ResetSettings;
        public string _LogsFilePath;
        public string _BufferSize;
        public bool _ConnectionMode;
        public ObservableCollection<string> _AvailableFolders = new ObservableCollection<string>();
        public RelayCommand<object> _DeletePositionFromList { get; set; }
        public ICommand _AddPositionToList;

    }
}
