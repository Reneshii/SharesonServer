using Shareson.Support;
using SharesonServer.Model.MainMenu;
using SharesonServer.Repository;
using SharesonServer.View.ControlsView;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SharesonServer.ViewModel
{
    public class MainMenuViewModel : Property_Changed
    {
        private MainMenuModel model;
        private MainMenuRepository repository;
        Task Task_CountConnectedClients;
        bool RepeatCountConnectedClients_Task;

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
        public ICommand ShutDownServer
        {
            get
            {
                if (model._shutDownServer == null)
                {
                    model._shutDownServer = new RelayCommand(p => CanTurnOffServer, p =>
                    {
                        CanTurnOnServer = repository.StopServer(); //enables button for turning on server again
                        CanTurnOffServer = false; //disable button for turning off server
                        RepeatCountConnectedClients_Task = false; // that ends checking loop
                    });
                }
                return model._shutDownServer;
            }
            set { }
        }
        public ICommand StartServer
        {
            get
            {
                if (model._startServer == null)
                {
                    model._startServer = new RelayCommand(p => CanTurnOnServer, async p =>
                    {
                        
                        MainMenuViewControlContent = new LoadingWindowControl();
                        if(await repository.sql.StartSQL() == true)
                        {
                            CanTurnOffServer = repository.RunServer();
                            CanTurnOnServer = false;
                            Task_CountConnectedClients.Start();
                        }
                        else
                        {
                            //if not, then allow to proceed but without login on a client side and without sql server functions
                            //return info to client that server is offline but can use it locally and limited by off functions
                            //UpdateSQLStatus();
                        }
                        MainMenuViewControlContent = null;
                    });
                }
                return model._startServer;
            }
            set { }
        }
        public object MainMenuViewControlContent
        {
            get
            {
                return model._MainMenuViewControlContent;
            }
            set
            {
                model._MainMenuViewControlContent = value;
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

        public MainMenuViewModel()
        {
            Initialize();
        }

        private void Initialize()
        {
            model = new MainMenuModel();
            repository = new MainMenuRepository();

            InitializeTasks();

            CanTurnOnServer = true;
            CanTurnOffServer = false;
            RepeatCountConnectedClients_Task = true;
        }

        private void InitializeTasks()
        {
            Task_CountConnectedClients = new Task(() =>
            {
                while (RepeatCountConnectedClients_Task == true)
                {
                    ConnectedClients = repository.ConnectedUsers();
                    Thread.Sleep(3000);
                }
            });
        }

    }
}
