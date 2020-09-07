
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
            ServersSettingsModel model = new ServersSettingsModel();
            model._AvailableFolders = Properties.Settings.Default.AvailableFoldersModel;
            model._LogsFilePath = Properties.Settings.Default.LogsFilePath;

            return model;
        }
        public void SaveSettings(ServersSettingsModel model)
        {
            Properties.Settings.Default.LogsFilePath = model._LogsFilePath;
            Properties.Settings.Default.AvailableFoldersModel = model._AvailableFolders;
            Properties.Settings.Default.Save();
        }
    }
}
