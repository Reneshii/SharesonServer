using Shareson.Support;
using SharesonServer.Model.ForControls;
using SharesonServer.Model.Support;
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

        public ObservableCollection<AvailableFoldersModel> AvailableFolders
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
            AvailableFolders = new ObservableCollection<AvailableFoldersModel>();

            model = repository.LoadSettings();
        }

        private void DeleteFile_execute(object obj)
        {
            try
            {
                var listPosition = (AvailableFoldersModel)obj;
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
                    AvailableFolders.Add(new AvailableFoldersModel()
                    {
                        PathToFolder = path.SelectedPath + @"\",
                    });
                }
            }
            catch(Exception e)
            {
                log.Add(e.ToString());
            }
            
        }
    }
}
