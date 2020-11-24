using SharesonServer.Model.ForControls;

namespace SharesonServer.Repository
{
    public class ServerSettingsRepository
    {
        public ServerSettingsRepository()
        {

        }

        public ServersSettingsModel LoadSettings()
        {
            ServersSettingsModel model = new ServersSettingsModel()
            {
                _AvailableFolders = new System.Collections.ObjectModel.ObservableCollection<string>()
            };
            if (Properties.Settings.Default.AvailableFoldersModel != null)
            {
                foreach (var item in Properties.Settings.Default.AvailableFoldersModel)
                {
                    model._AvailableFolders.Add(item);
                }
            }
            if(!string.IsNullOrEmpty(Properties.Settings.Default.LogsFilePath))
            {
                model._LogsFilePath = Properties.Settings.Default.LogsFilePath;
            }
            if(!string.IsNullOrEmpty(Properties.Settings.Default.BufferSize))
            {
                model._BufferSize = Properties.Settings.Default.BufferSize;
            }


            model._ConnectionMode = Properties.Settings.Default.ConnectionMode;

            return model;
        }
        public void SaveSettings(ServersSettingsModel model)
        {
            Properties.Settings.Default.AvailableFoldersModel = new System.Collections.Specialized.StringCollection();
            foreach (var item in model._AvailableFolders)
            {
                Properties.Settings.Default.AvailableFoldersModel.Add(item);
            }

            Properties.Settings.Default.BufferSize = model._BufferSize;
            Properties.Settings.Default.ConnectionMode = model._ConnectionMode;
            Properties.Settings.Default.LogsFilePath = model._LogsFilePath;

            Properties.Settings.Default.Save();
        }
    }
}
