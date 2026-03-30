using Dartsmanager.Data;
using Dartsmanager.Models;
using Dartsmanager.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using BC = BCrypt.Net.BCrypt;

namespace Dartsmanager.Views.Windows
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        public User? UserLoggedIn = null;

        public LoginWindow()
        {
            InitializeComponent();
        }

        private void BT_Login_Click(object sender, RoutedEventArgs e)
        {
            var gebruiker = UserService.GetUserFromName(TB_Username.Text);
            if (gebruiker == null)
            {
                MessageBox.Show("Dit is geen geldige username!"); return;
            }
            string password = TB_Wachtwoord.Password;
            if (BC.Verify(password, gebruiker.WachtwoordHash))
            {
                UserLoggedIn = gebruiker;
                Close();
            }
            else
            {
                MessageBox.Show("Ongeldig wachtwoord!"); return;

            }
        }

        private void BT_Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void BT_Registreer_Click(object sender, RoutedEventArgs e)
        {
            var RegistreerScherm = new UserWindow(UserLoggedIn);
            RegistreerScherm.ShowDialog();
            // Loginscherm tonen met geregistreerde username


        }
    }
}
