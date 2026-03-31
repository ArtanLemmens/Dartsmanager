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
        private User? _UserLoggedIn = null;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void BT_Home_Click(object sender, RoutedEventArgs e)
        {
            
        }

        private void BT_Profiel_Click(object sender, RoutedEventArgs e)
        {
            Frame_Pagina.Navigate(new UserPage(_UserLoggedIn)); 
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
            // LOGOUT indien reeds ingelogd
            if (_UserLoggedIn != null)
            {
                _UserLoggedIn = null;
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
                _UserLoggedIn = LoginScherm.UserLoggedIn;
                // Knopcontent wijzigen naar de username als login = succesvol
                if (_UserLoggedIn != null)
                {
                    TB_Username.Text = _UserLoggedIn.Username;
                    TB_Username.Visibility = Visibility.Visible;
                    BT_Login.Content = "LOGOUT";
                    BT_Login.Background = new SolidColorBrush(Colors.IndianRed);
                }
            }
        }

        private void BT_Registreer_Click(object sender, RoutedEventArgs e)
        {
            var RegistreerScherm = new UserWindow();
            RegistreerScherm.ShowDialog();
            // Loginscherm tonen met geregistreerde username

        }
    }
}