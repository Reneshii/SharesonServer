using Shareson.Support;
using SharesonServer.Interface;
using SharesonServer.Model.ForViews;
using SharesonServer.Model.Support;
using SharesonServer.Repository;
using SharesonServer.View.ControlsView;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SharesonServer.ViewModel.ControlsViewModel
{
    public class StartMenuControlViewModel : Property_Changed
    {
        private StartMenuModel model;
        private IMainMenuRepositoryFunctionsWithoutLimit repositoryFull;
        private IMainMenuRepositoryFunctionsWithLimit repositoryLimited;
        private InfoLog log = new InfoLog($@"C:\Users\Reneshi\Downloads", "ServerLogs.txt");
         
        private Task Task_CountConnectedClients;

        #region ICommand 
        public ICommand StartServer
        {
            get
            {
                if (model._startServer == null)
                {
                    model._startServer = new RelayCommand(p => CanTurnOnServer, async p =>
                    {
                        try
                        {
                            repositoryFull = new StartMainMenuRepository();
                            StartMenuControlContent = new LoadingWindowControl();
                            CanTurnOnServer = false;

                            if (await repositoryFull.sql.StartSQL() == true)
                            {
                                CanTurnOffServer = repositoryFull.RunServer();
                                InitializeAndRunTasksForFullRepository();

                            }
                            else
                            {
                                repositoryLimited = new StartMainMenuRepository();
                                repositoryFull = null;

                                ServerWithoutSQL = repositoryLimited.RunServerWithLimitedFunctions();
                            }

                            CanTurnOffServer = true;
                            StartMenuControlContent = null;
                        }
                        catch (Exception e)
                        {
                            log.Add(e.ToString());
                            CanTurnOnServer = true;
                            CanTurnOffServer = false;
                        }
                    });
                }
                return model._startServer;
            }
            set { }
        }
        public ICommand ShutDownServer
        {
            get
            {
                try
                {
                    if (model._shutDownServer == null)
                    {
                        model._shutDownServer = new RelayCommand(p => CanTurnOffServer, p =>
                        {
                            CanTurnOffServer = false;
                            if (repositoryFull != null)
                            {
                                CanTurnOnServer = repositoryFull.StopServer();
                            }
                            else
                            {
                                CanTurnOnServer = repositoryLimited.StopServer();
                            }

                            CloseAllPendingTasks();
                        });
                    }
                    return model._shutDownServer;
                }
                catch(Exception e)
                {
                    log.Add(e.ToString());
                    CanTurnOnServer = false;
                    CanTurnOffServer = true;
                    return model._shutDownServer;
                }
            }
            set { }
        }
        #endregion

        #region Properties
        public ObservableCollection<FullClientInfoModel> UsersSource
        {
            get
            {
                return model._UsersSource;
            }
            set
            {
                model._UsersSource = value;
                NotifyPropertyChanged();
            }
        }
        public object StartMenuControlContent
        {
            get
            {
                return model._StartMenuControlContent;
            }
            set
            {
                model._StartMenuControlContent = value;
                NotifyPropertyChanged();
            }
        }
        public bool CanTurnOffServer
        {
            get
            {
                return model._CanTurnOffServer;
            }
            set
            {
                model._CanTurnOffServer = value;
                NotifyPropertyChanged();
            }
        }
        public bool ServerWithoutSQL
        {
            get
            {
                return model._ServerWithoutSQL;
            }
            set
            {
                model._ServerWithoutSQL = value;
                NotifyPropertyChanged();
            }
        }
        public bool CanTurnOnServer
        {
            get
            {
                return model._CanTurnOnServer;
            }
            set
            {
                model._CanTurnOnServer = value;
                NotifyPropertyChanged();
            }
        }
        public int ConnectedClients
        {
            get
            {
                return model._ConnectedClients;
            }
            set
            {
                model._ConnectedClients = value;
                NotifyPropertyChanged();
            }
        }
        #endregion

        public StartMenuControlViewModel()
        {
            model = new StartMenuModel();
            UsersSource = new ObservableCollection<FullClientInfoModel>();

            CanTurnOnServer = true;
            CanTurnOffServer = false;
        }

        private void InitializeAndRunTasksForFullRepository()
        {
            Task_CountConnectedClients = new Task(() =>
            {
                if(repositoryFull != null)
                {
                    repositoryFull.RepeatCountConnectedClients_Task = true;
                }
                else
                {
                    repositoryLimited.RepeatCountConnectedClients_Task = true;
                }
                
                CountConnectedClients();
            });


            Task_CountConnectedClients.Start();
        }
        private void InitializeAndRunTasksForLimitedRepository()
        {

        }

        private void CloseAllPendingTasks()
        {
            if(repositoryFull != null)
            {
                if (Task.WhenAll(Task_CountConnectedClients).IsCompleted)
                {
                    repositoryFull = null;
                }
            }
            else
            {
               
            }
        }

        private void CountConnectedClients()
        {
            if(repositoryFull != null)
            {
                while (repositoryFull.RepeatCountConnectedClients_Task == true)
                {
                    ConnectedClients = repositoryFull.ConnectedUsers();
                    var list = repositoryFull.UpdateUsersList();

                    App.Current.Dispatcher.Invoke((Action)delegate
                    {
                        UsersSource.Clear();
                        foreach (var item in list)
                        {
                            UsersSource.Add(item);
                        }
                    });
                    Thread.Sleep(3000);
                }
            }
            else
            {
                while (repositoryLimited.RepeatCountConnectedClients_Task == true)
                {
                    ConnectedClients = repositoryLimited.ConnectedUsers();
                    Thread.Sleep(3000);
                }
            }
        }
    }
}
