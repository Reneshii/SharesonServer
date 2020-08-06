using SharesonServer.ViewModel;
using System.Windows;


namespace SharesonServer.View
{
    /// <summary>
    /// Logika interakcji dla klasy MainMenu.xaml
    /// </summary>
    public partial class MainMenu : Window
    {
        MainMenuViewModel mainViewModel = new MainMenuViewModel();

        public MainMenu()
        {
            InitializeComponent();
            base.DataContext = mainViewModel;
            CenterScreen();
        }

        private void CenterScreen()
        {
            double screenWidth = System.Windows.SystemParameters.PrimaryScreenWidth;
            double screenHeight = System.Windows.SystemParameters.PrimaryScreenHeight;
            double windowWidth = this.Width;
            double windowHeight = this.Height;
            this.Left = (screenWidth / 2) - (windowWidth / 2) -150 ;
            this.Top = (screenHeight / 2) - (windowHeight / 2);
        }
    }
}
