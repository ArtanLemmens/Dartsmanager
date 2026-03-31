using Dartsmanager.Models;
using Dartsmanager.Services;
using Dartsmanager.Views.Windows;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using BC = BCrypt.Net.BCrypt;

namespace Dartsmanager.Views.Pages
{
    /// <summary>
    /// Interaction logic for UserPage.xaml
    /// </summary>
    public partial class UserPage : Page
    {
        private User? _actieve_gebruiker = null;

        public UserPage(User? actieve_gebruiker)
        {
            InitializeComponent();
            _actieve_gebruiker = actieve_gebruiker;
            SetActiveUserValues();
            LoadPlayerData();
        }

        private void SetActiveUserValues()
        {
            if (_actieve_gebruiker != null)
            {
                TB_NotLoggedIn.Visibility = Visibility.Collapsed;
                Grid_UserInfo.Visibility = Visibility.Visible;
                TB_Username.Text = _actieve_gebruiker.Username;
                CB_Spelers.SelectedValue = _actieve_gebruiker.PlayerId;
                CHB_Speler_Bevestigd.IsChecked = _actieve_gebruiker.PlayerIdBevestigd;
            }
            else
            {
                TB_NotLoggedIn.Visibility = Visibility.Visible;
                Grid_UserInfo.Visibility = Visibility.Collapsed;
            }
        }

        private void LoadPlayerData()
        {
            // enkel speler data tonen als de actieve speler ook is bevestigd door de admin!
            if (_actieve_gebruiker != null && _actieve_gebruiker.Player != null && _actieve_gebruiker.PlayerIdBevestigd == true)
            {
                Grid_PlayerInfo.Visibility = Visibility.Visible;
            }
            else
            {
                Grid_PlayerInfo.Visibility = Visibility.Collapsed;
            }
        }

        private void BT_Update_Name_Click(object sender, RoutedEventArgs e)
        {
            if (_actieve_gebruiker == null)
            {
                MessageBox.Show("Er is geen gebruiker ingelogd om up te daten"); return;
            }
            // Wachtwoord vragen om te bevestigen
            var WachtwoordScherm = new PasswordWindow(_actieve_gebruiker);
            WachtwoordScherm.ShowDialog();
            // Als het wachtwoord correct is, wijziging doorvoeren
            if (WachtwoordScherm._CorrectWachtwoord == true)
            {
                // Username ophalen en kijken of hij niet bestaat in een andere gebruiker
                string username = TB_Username.Text;
                if (username == _actieve_gebruiker.Username) // Username ongewijzigd => geen wijziging of melding nodig
                {
                    return;
                }
                if (UserService.CheckExistingName(username, _actieve_gebruiker.Id) == true) // Username bestaat al in database
                {
                    MessageBox.Show("Deze username bestaat al.\nGelieve een andere naam te kiezen."); return;
                }
                _actieve_gebruiker.Username = username;
                // Aanpassen in de database
                UserService.Update(_actieve_gebruiker);
                MessageBox.Show($"Uw username werd gewijzigd naar: {username}.");
            }
        }

        private void BT_Update_Wachtwoord_Click(object sender, RoutedEventArgs e)
        {
            if (_actieve_gebruiker == null)
            {
                MessageBox.Show("Er is geen gebruiker ingelogd om up te daten"); return;
            }
            // Wachtwoord vragen om te bevestigen
            var WachtwoordScherm = new PasswordWindow(_actieve_gebruiker);
            WachtwoordScherm.ShowDialog();
            // Als het wachtwoord correct is, wijziging doorvoeren
            if (WachtwoordScherm._CorrectWachtwoord == true)
            {
                string nieuw_password = TB_Wachtwoord.Password;
                _actieve_gebruiker.WachtwoordHash = BC.HashPassword(nieuw_password);
                // Aanpassen in de database
                UserService.Update(_actieve_gebruiker);
                MessageBox.Show($"Uw wachtwoord werd gewijzigd.");
            }
        }

        private void CB_Spelers_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_actieve_gebruiker == null)
            {
                MessageBox.Show("Er is geen gebruiker ingelogd om up te daten"); return;
            }
            // Wachtwoord vragen om te bevestigen
            var WachtwoordScherm = new PasswordWindow(_actieve_gebruiker);
            WachtwoordScherm.ShowDialog();
            // Als het wachtwoord correct is, wijziging doorvoeren
            if (WachtwoordScherm._CorrectWachtwoord == true)
            {
                // Speler ophalen uit combobox indien er 1 is geselcteerd
                if (CB_Spelers.SelectedItem is Player speler)
                {
                    if (UserService.CheckExistingPlayerLink(speler, _actieve_gebruiker.Id) == true)
                    {
                        MessageBox.Show("Deze speler is al gelinkt aan een andere gebruiker!\nNeem contact op met onze admins indien u dit wilt laten wijzigen."); return;
                    }
                    _actieve_gebruiker.PlayerId = speler.Id;
                    _actieve_gebruiker.PlayerIdBevestigd = false;
                }
                // Aanpassen in de database
                UserService.Update(_actieve_gebruiker);
                MessageBox.Show($"Uw aanvraag tot spelerskoppeling werd ingediend bij onze admins.");
                LoadPlayerData();
            }
        }

        private void BT_Create_Player_Click(object sender, RoutedEventArgs e)
        {
            
        }

        private void BT_Player_Data_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BT_Player_Tornooien_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BT_Player_Wedstrijden_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BT_Player_Statistieken_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
