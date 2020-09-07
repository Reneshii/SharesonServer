using Shareson.Support;
using SharesonServer.Model.Support;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace SharesonServer.Model.ForControls
{
    public class ServersSettingsModel
    {
        public ICommand _SaveSettings;
        public ICommand _ResetSettings;
        public string _LogsFilePath;
        public ObservableCollection<AvailableFoldersModel> _AvailableFolders { get; set; }
        public RelayCommand<object> _DeletePositionFromList { get; set; }
        public RelayCommand<object> _AddPositionToList;
    }
}
