using System.Text;
using System.Windows;
using Dartsmanager.Views.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Dartsmanager
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void BT_Home_Click(object sender, RoutedEventArgs e)
        {
            //if (_LoggedIn == true)
            //{
            //    frame.Navigate(new AdminPage(_dbPath)); // Admin waarde meegeven
            //}
        }

        private void BT_Profiel_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BT_Speler_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BT_Tornooi_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BT_Wedstrijd_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BT_Statistiek_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BT_Login_Click(object sender, RoutedEventArgs e)
        {
            // Toon loginscherm 
            var LoginScherm = new LoginWindow();
            LoginScherm.ShowDialog();
            //_UserLoggedIn = LoginScherm.UserLoggedIn
            //if (_UserLoggedIn != null)
            //{
            //    BT_Login.Content = _UserLoggedIn.Username;
            //}
        }

        private void BT_Registreer_Click(object sender, RoutedEventArgs e)
        {
            var RegistreerScherm = new UserWindow();
            RegistreerScherm.ShowDialog();
        }
    }
}