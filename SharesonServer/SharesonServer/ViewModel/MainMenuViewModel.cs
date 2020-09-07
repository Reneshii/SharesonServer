using Shareson.Support;
using SharesonServer.Model.MainMenu;
using SharesonServer.ViewModel.ControlsViewModel;
using System.Windows.Input;

namespace SharesonServer.ViewModel
{
    public class MainMenuViewModel : Property_Changed
    {
        private MainMenuModel model;
        private StartMenuControlViewModel startMenuControl;
        private ServerSettingsControlViewModel serversSettingsControl;
        private AccountsSettingsControlViewModel accountsSettingsControl;

        #region Commands
        public ICommand MenuStartBtn
        {
            get
            {
                if(model._MenuStartBtn == null)
                {
                    model._MenuStartBtn = new RelayCommand(p => true, p =>
                    {
                        MainMenuViewControlContent = startMenuControl;
                    });
                }
                return model._MenuStartBtn;
            }
            set { }
        }
        public ICommand MenuServersSettingsBtn
        {
            get
            {
                if (model._MenuServersSettingsBtn == null)
                {
                    model._MenuServersSettingsBtn = new RelayCommand(p => true, p =>
                    {
                        MainMenuViewControlContent = serversSettingsControl;
                    });
                }
                return model._MenuServersSettingsBtn;
            }
            set { }
        }
        public ICommand MenuAccountsSettingsBtn
        {
            get
            {
                if (model._MenuAccountsSettingsBtn == null)
                {
                    model._MenuAccountsSettingsBtn = new RelayCommand(p => true, p =>
                    {
                        MainMenuViewControlContent = accountsSettingsControl;
                    });
                }
                return model._MenuAccountsSettingsBtn;
            }
            set { }
        }
        #endregion

        #region Properties
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
        #endregion

        public MainMenuViewModel()
        {
            Initialize();
        }

        private void Initialize()
        {
            model = new MainMenuModel();
            startMenuControl = new StartMenuControlViewModel();
            serversSettingsControl = new ServerSettingsControlViewModel();
            accountsSettingsControl = new AccountsSettingsControlViewModel();
        }
    }
}
