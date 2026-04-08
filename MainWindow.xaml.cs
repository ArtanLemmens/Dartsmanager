using System.Text;
using System.Windows;
using Dartsmanager.Views.Windows;
using Dartsmanager.Models;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Dartsmanager.Views.Pages;

namespace Dartsmanager
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private User? _actieve_gebruiker = null;

        public MainWindow()
        {
            InitializeComponent();
            ShowAdminButton();
        }

        private void ShowAdminButton()
        {
            if (_actieve_gebruiker != null && _actieve_gebruiker.IsAdmin == true)
            {
                BT_Admin.Visibility = Visibility.Visible;
            }
            else
            {
                BT_Admin.Visibility = Visibility.Collapsed;
            }
        }

        private void BT_Login_Click(object sender, RoutedEventArgs e)
        {
            // LOGOUT indien reeds ingelogd
            if (_actieve_gebruiker != null)
            {
                _actieve_gebruiker = null;
                TB_Username.Text = "";
                TB_Username.Visibility = Visibility.Collapsed;
                BT_Login.Content = "LOGIN";
                BT_Login.ClearValue(Button.BackgroundProperty);
            }
            else
            {
                // Toon loginscherm 
                var LoginScherm = new LoginWindow();
                LoginScherm.ShowDialog();
                _actieve_gebruiker = LoginScherm.UserLoggedIn;
                // Knopcontent wijzigen naar de username als login = succesvol
                if (_actieve_gebruiker != null)
                {
                    TB_Username.Text = _actieve_gebruiker.Username;
                    TB_Username.Visibility = Visibility.Visible;
                    BT_Login.Content = "LOGOUT";
                    BT_Login.Background = new SolidColorBrush(Colors.IndianRed);
                }
            }
            Frame_Pagina.Navigate(new HomePage(_actieve_gebruiker));
            ShowAdminButton();
        }

        private void BT_Registreer_Click(object sender, RoutedEventArgs e)
        {
            var RegistreerScherm = new UserWindow();
            RegistreerScherm.ShowDialog();
            // Loginscherm tonen met geregistreerde username

        }

        private void BT_Home_Click(object sender, RoutedEventArgs e)
        {
            Frame_Pagina.Navigate(new HomePage(_actieve_gebruiker));
        }

        private void BT_Profiel_Click(object sender, RoutedEventArgs e)
        {
            Frame_Pagina.Navigate(new UserPage(_actieve_gebruiker, _actieve_gebruiker, Frame_Pagina)); 
        }

        private void BT_Speler_Click(object sender, RoutedEventArgs e)
        {
            Frame_Pagina.Navigate(new PlayerOverview(_actieve_gebruiker, Frame_Pagina));
        }

        private void BT_Tornooi_Click(object sender, RoutedEventArgs e)
        {
            Frame_Pagina.Navigate(new TournamentOverview(_actieve_gebruiker, Frame_Pagina));
        }

        private void BT_Wedstrijd_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BT_Statistiek_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BT_Admin_Click(object sender, RoutedEventArgs e)
        {
            Frame_Pagina.Navigate(new AdminPage(_actieve_gebruiker));
        }
    }
}