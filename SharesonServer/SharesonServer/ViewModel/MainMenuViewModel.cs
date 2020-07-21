using Shareson.Support;
using SharesonServer.Model.MainMenu;
using SharesonServer.Repository;
using SharesonServer.Repository.SupportFunctions;
using SharesonServer.View.ControlsView;
using System.Windows.Input;

namespace SharesonServer.ViewModel
{
    public class MainMenuViewModel : Property_Changed
    {
        private MainMenuModel model;
        private MainMenuRepository repository;
        private SqlHelper sql;

        public bool IsServerWorks
        {
            get
            {
                return model._isServerWorks;
            }
            set
            {
                model._isServerWorks = value;
                NotifyPropertyChanged();
            }
        }
        public ICommand ShutDownServer
        {
            get
            {
                if (model._shutDownServer == null)
                {
                    //mainMenuModel._startServer = new RelayCommand(p => IsEnableStartServer, p => repository.CloseAllSockets());
                }
                return model._shutDownServer;
            }
            set { }
        }
        public ICommand StartServer
        {
            get
            {
                if(model._startServer == null)
                {
                    IsServerWorks = IsServerWorks == false ? true : false;
                    model._startServer = new RelayCommand(p => IsServerWorks, async p =>
                    {
                        MainMenuViewControlContent = new LoadingWindowControl();
                        if(await sql.StartSQL() == true)
                        {
                            IsServerWorks = repository.RunServer();
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

        public MainMenuViewModel()
        {
            Initialize();
        }

        private void Initialize()
        {
            model = new MainMenuModel();
            repository = new MainMenuRepository();
            sql = new SqlHelper();
        }

    }
}
