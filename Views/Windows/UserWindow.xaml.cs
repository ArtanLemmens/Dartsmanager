using Dartsmanager.Services;
using Dartsmanager.Models;
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
    /// Interaction logic for UserWindow.xaml
    /// </summary>
    public partial class UserWindow : Window
    {
        public UserWindow()
        {
            InitializeComponent();
        }
        private void BT_Create_Click(object sender, RoutedEventArgs e)
        {
            // Username ophalen en kijken of hij niet bestaat
            string username = TB_Username.Text;
            if (UserService.CheckExistingName(username) == true)
            {
                MessageBox.Show("Deze username bestaat al.\nGelieve een andere naam te kiezen."); return;
            }
            // Wachtwoord ophalen en kijken of het uniek is
            string wachtwoord = TB_Wachtwoord.Password;
            if (wachtwoord == "")
            {
                MessageBox.Show("Gelieve een wachtwoord in te vullen."); return;
            }

            User nieuwe_gebruiker = new User
            {
                Username = username,
                WachtwoordHash = BC.HashPassword(wachtwoord)
            };

            // Speler ophalen uit combobox indien er 1 is geselecteerd
            if (CB_Spelers.SelectedItem is Player speler)
            {
                if (UserService.CheckExistingPlayerLink(speler) == true)
                {
                    MessageBox.Show("Deze speler is al gelinkt aan een andere gebruiker!\nNeem contact op met onze admins indien u dit wilt laten wijzigen."); return;
                }
                nieuwe_gebruiker.PlayerId = speler.Id;                
            }
            nieuwe_gebruiker.PlayerIdBevestigd = false;

            // Kijken of er admins bestaan en anders voorstellen om een admin aan te maken
            nieuwe_gebruiker.IsAdmin = false;
            if (UserService.CountAdmins() == 0)
            {
                MessageBoxResult result = MessageBox.Show($"Er bestaan nog geen admins in het systeem. Wenst u deze gebruiker als admin aan te maken?",
                                                  "Bevestig admin",
                                                  MessageBoxButton.YesNo,
                                                  MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    nieuwe_gebruiker.IsAdmin = true;
                }
            }

            // Gebruiker toevoegen
            UserService.Add(nieuwe_gebruiker);
            Close();
        }

        private void BT_Create_Player_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BT_Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        
    }
}
