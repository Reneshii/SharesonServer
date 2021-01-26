using Shareson.Support;
using SharesonServer.Model.ForControls;
using SharesonServer.Repository;
using System;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace SharesonServer.ViewModel.ControlsViewModel
{
    public class ServerSettingsControlViewModel : Property_Changed
    {
        InfoLog log;
        ServersSettingsModel model;
        ServerSettingsRepository repository;

        #region Commands
        public RelayCommand<object> DeletePositionFromList
        {
            get
            {
                if (model._DeletePositionFromList == null)
                {
                    model._DeletePositionFromList = new RelayCommand<object>(DeleteFile_execute, f => true);
                }
                return model._DeletePositionFromList;
            }
            set { }
        } 
        public ICommand AddPositionToList
        {
            get
            {
                if (model._AddPositionToList == null)
                {
                    model._AddPositionToList = new RelayCommand(f => true, f => { AddFile_execute(); });
                }
                return model._AddPositionToList;
            }
            set { }
        }
        public ICommand SaveSettings
        {
            get
            {
                if(model._SaveSettings == null)
                {
                    model._SaveSettings = new RelayCommand(p => true, p =>
                    {
                        repository.SaveSettings(model);
                    });
                }
                return model._SaveSettings;
            }
            set { }
        }
        public ICommand ResetSettings
        {
            get
            {
                return model._ResetSettings;
            }
            set
            {
                model._ResetSettings = value;
                NotifyPropertyChanged();
            }
        }
        #endregion

        #region Properties
        public string LogsFilePath
        {
            get
            {
                return model._LogsFilePath;
            }
            set
            {
                model._LogsFilePath = value;
                NotifyPropertyChanged();
            }
        }
        public int Port
        {
            get
            {
                return model._Port;
            }
            set
            {
                model._Port = value;
                NotifyPropertyChanged();
            }
        }
        public int BufferSize
        {
            get
            {
                return model._BufferSize;
            }
            set
            {
                //var arrayToCheck = System.Text.Encoding.ASCII.GetBytes(value);
                //var destination = Array.FindAll(arrayToCheck, f => (f > 47 && f < 58));

                //model._BufferSize = System.Text.Encoding.ASCII.GetString(destination);
                model._BufferSize = value;
                NotifyPropertyChanged();
            }
        }
        public bool ConnectionMode
        {
            get
            {
                return model._WLAN;
            }
            set
            {
                model._WLAN = value;
                NotifyPropertyChanged();
            }
        }

        public ObservableCollection<string> AvailableFolders
        {
            get
            {
                return model._AvailableFolders;
            }
            set
            {
                model._AvailableFolders = value;
                NotifyPropertyChanged();
            }
        }
        #endregion

        public ServerSettingsControlViewModel()
        {
            model = new ServersSettingsModel();
            repository = new ServerSettingsRepository();
            log = new InfoLog(Properties.Settings.Default.LogsFilePath);
            AvailableFolders = new ObservableCollection<string>();

            model = repository.LoadSettings();
        }

        private void DeleteFile_execute(object obj)
        {
            try
            {
                var listPosition = (string)obj;
                AvailableFolders.Remove(listPosition);
            }
            catch(Exception e)
            {
                log.Add(e.ToString());
            }
        }

        private void AddFile_execute()
        {
            try
            {
                var path = new System.Windows.Forms.FolderBrowserDialog();
                path.ShowDialog();
                if (!string.IsNullOrEmpty(path.SelectedPath))
                {
                    AvailableFolders.Add(path.SelectedPath + @"\");
                }
            }
            catch(Exception e)
            {
                log.Add(e.ToString());
            }
            
        }
    }
}
