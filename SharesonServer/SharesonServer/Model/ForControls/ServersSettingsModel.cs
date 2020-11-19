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
        public ObservableCollection<AvailableFoldersModel> _AvailableFolders = new ObservableCollection<AvailableFoldersModel>();
        public RelayCommand<object> _DeletePositionFromList { get; set; }
        public ICommand _AddPositionToList;
    }
}
