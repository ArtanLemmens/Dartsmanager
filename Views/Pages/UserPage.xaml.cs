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
        private User? _geselecteerde_gebruiker = null;
        private Frame _frame;

        public UserPage(User? actieve_gebruiker, User? geselecteerde_gebruiker, Frame frame)
        {
            InitializeComponent();
            _actieve_gebruiker = actieve_gebruiker;
            _geselecteerde_gebruiker = geselecteerde_gebruiker;
            _frame = frame;
            SetActiveUserValues();
            LoadPlayerData();
            BindData();
        }
        private void BindData()
        {
            List<Player> spelers = PlayerService.GetAll();
            CB_Spelers.ItemsSource = spelers;
        }

        private void SetActiveUserValues()
        {
            if (_geselecteerde_gebruiker != null)
            {
                TB_NotLoggedIn.Visibility = Visibility.Collapsed;
                Grid_UserInfo.Visibility = Visibility.Visible;
                TB_Username.Text = _geselecteerde_gebruiker.Username;
                CB_Spelers.SelectedValue = _geselecteerde_gebruiker.PlayerId;
                CHB_Speler_Bevestigd.IsChecked = _geselecteerde_gebruiker.PlayerIdBevestigd;
                BT_Update_Speler_Bevestigd.Visibility = Visibility.Collapsed;
                CHB_IsAdmin.IsChecked = _geselecteerde_gebruiker.IsAdmin;
                BT_IsAdmin_Bevestigd.Visibility = Visibility.Collapsed;
            }
            else
            {
                TB_NotLoggedIn.Visibility = Visibility.Visible;
                Grid_UserInfo.Visibility = Visibility.Collapsed;
            }
            // Indien admin, checkbox vrijgeven
            if (_actieve_gebruiker != null && _actieve_gebruiker.IsAdmin == true)
            {
                CHB_Speler_Bevestigd.IsEnabled = true;
                BT_Update_Speler_Bevestigd.Visibility = Visibility.Visible;
                CHB_IsAdmin.IsEnabled = true;
                BT_IsAdmin_Bevestigd.Visibility = Visibility.Visible;
            }
        }

        private void LoadPlayerData()
        {
            // enkel speler data tonen als de actieve speler ook is bevestigd door de admin!
            if (_geselecteerde_gebruiker != null && _geselecteerde_gebruiker.PlayerId != null && _geselecteerde_gebruiker.PlayerIdBevestigd == true)
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
            if (_geselecteerde_gebruiker == null)
            {
                MessageBox.Show("Er is geen gebruiker om up te daten"); return;
            }
            // Wachtwoord vragen om te bevestigen
            var WachtwoordScherm = new PasswordWindow(_geselecteerde_gebruiker);
            WachtwoordScherm.ShowDialog();
            // Als het wachtwoord correct is, wijziging doorvoeren
            if (WachtwoordScherm._CorrectWachtwoord == true)
            {
                // Username ophalen en kijken of hij niet bestaat in een andere gebruiker
                string username = TB_Username.Text;
                if (username == _geselecteerde_gebruiker.Username) // Username ongewijzigd => geen wijziging of melding nodig
                {
                    return;
                }
                if (UserService.CheckExistingName(username, _geselecteerde_gebruiker.Id) == true) // Username bestaat al in database
                {
                    MessageBox.Show("Deze username bestaat al.\nGelieve een andere naam te kiezen."); return;
                }
                _geselecteerde_gebruiker.Username = username;
                // Aanpassen in de database
                UserService.Update(_geselecteerde_gebruiker);
                MessageBox.Show($"Uw username werd gewijzigd naar: {username}.");
            }
        }

        private void BT_Update_Wachtwoord_Click(object sender, RoutedEventArgs e)
        {
            if (_geselecteerde_gebruiker == null)
            {
                MessageBox.Show("Er is geen gebruiker om up te daten"); return;
            }
            // Wachtwoord vragen om te bevestigen
            var WachtwoordScherm = new PasswordWindow(_geselecteerde_gebruiker);
            WachtwoordScherm.ShowDialog();
            // Als het wachtwoord correct is, wijziging doorvoeren
            if (WachtwoordScherm._CorrectWachtwoord == true)
            {
                string nieuw_password = TB_Wachtwoord.Password;
                _geselecteerde_gebruiker.WachtwoordHash = BC.HashPassword(nieuw_password);
                // Aanpassen in de database
                UserService.Update(_geselecteerde_gebruiker);
                MessageBox.Show($"Uw wachtwoord werd gewijzigd.");
            }
        }

        private void BT_Update_Player_Click(object sender, RoutedEventArgs e)
        {
            if (_geselecteerde_gebruiker == null)
            {
                MessageBox.Show("Er is geen gebruiker om up te daten"); return;
            }            
            // Speler ophalen uit combobox indien er 1 is geselcteerd
            if (CB_Spelers.SelectedItem is Player speler)
            {
                // Wachtwoord vragen om te bevestigen
                var WachtwoordScherm = new PasswordWindow(_geselecteerde_gebruiker);
                WachtwoordScherm.ShowDialog();
                // Als het wachtwoord correct is, wijziging doorvoeren
                if (WachtwoordScherm._CorrectWachtwoord == true)
                {
                    if (UserService.CheckExistingPlayerLink(speler, _geselecteerde_gebruiker.Id) == true)
                    {
                        MessageBox.Show("Deze speler is al gelinkt aan een andere gebruiker!\nNeem contact op met onze admins indien u dit wilt laten wijzigen."); return;
                    }
                    _geselecteerde_gebruiker.PlayerId = speler.Id;
                    // Indien de actieve gebruiker een admin is mag de speler ineens bevestigd worden
                    if (_actieve_gebruiker != null && _actieve_gebruiker.IsAdmin == true)
                    {
                        _geselecteerde_gebruiker.PlayerIdBevestigd = true;
                    }
                    else
                    {
                        _geselecteerde_gebruiker.PlayerIdBevestigd = false;
                        MessageBox.Show($"Uw aanvraag tot spelerskoppeling werd ingediend bij onze admins.");
                    }
                    // Aanpassen in de database
                    UserService.Update(_geselecteerde_gebruiker);                    
                    // Spelerdata laden
                    LoadPlayerData();
                }
            }
        }
        private void BT_Update_Speler_Bevestigd_Click(object sender, RoutedEventArgs e)
        {
            if (_geselecteerde_gebruiker == null)
            {
                MessageBox.Show("Er is geen gebruiker om up te daten"); return;
            }
            if (_actieve_gebruiker != null && _actieve_gebruiker.IsAdmin == false)
            {
                MessageBox.Show("U heeft geen rechten om deze waarde te wijzigen"); return;
            }
            if (CHB_Speler_Bevestigd.IsChecked == true)
            {
                _geselecteerde_gebruiker.PlayerIdBevestigd = true;
            }
            else
            {
                _geselecteerde_gebruiker.PlayerIdBevestigd = false;
            }
            UserService.Update(_geselecteerde_gebruiker);
            MessageBox.Show($"De spelerskoppeling is bevestigd.");
            // Spelerdata laden
            LoadPlayerData();
        }

        private void BT_IsAdmin_Bevestigd_Click(object sender, RoutedEventArgs e)
        {
            if (_geselecteerde_gebruiker == null)
            {
                MessageBox.Show("Er is geen gebruiker om up te daten"); return;
            }
            if (_actieve_gebruiker != null && _actieve_gebruiker.IsAdmin == false)
            {
                MessageBox.Show("U heeft geen rechten om deze waarde te wijzigen"); return;
            }
            if (CHB_IsAdmin.IsChecked == true)
            {
                _geselecteerde_gebruiker.IsAdmin = true;
            }
            else
            {
                _geselecteerde_gebruiker.IsAdmin = false;
            }
            UserService.Update(_geselecteerde_gebruiker);
            MessageBox.Show($"De spelers adminrechten zijn gewijzigd.");
            // Spelerdata laden
            LoadPlayerData();
        }

        private void BT_Create_Player_Click(object sender, RoutedEventArgs e)
        {
            // Toon spelerscherm 
            var SpelerScherm = new PlayerWindow();
            SpelerScherm.ShowDialog();
        }

        private void BT_Player_Data_Click(object sender, RoutedEventArgs e)
        {
            if (_geselecteerde_gebruiker != null && _geselecteerde_gebruiker.PlayerId != null && _geselecteerde_gebruiker.PlayerIdBevestigd == true)
            {
                var speler = PlayerService.GetPlayerFromId((int)_geselecteerde_gebruiker.PlayerId);
                if (speler != null)
                {
                    Frame_Player.Navigate(new PlayerPage(_actieve_gebruiker, speler, Frame_Player));
                }
            }
        }

        private void BT_Player_Tornooien_Click(object sender, RoutedEventArgs e)
        {
            if (_geselecteerde_gebruiker != null && _geselecteerde_gebruiker.PlayerId != null && _geselecteerde_gebruiker.PlayerIdBevestigd == true)
            {
                Frame_Player.Navigate(new TournamentOverview(_actieve_gebruiker, Frame_Player, _geselecteerde_gebruiker.Player));
            }
        }

        private void BT_Player_Wedstrijden_Click(object sender, RoutedEventArgs e)
        {
            if (_geselecteerde_gebruiker != null && _geselecteerde_gebruiker.PlayerId != null && _geselecteerde_gebruiker.PlayerIdBevestigd == true)
            {
                Frame_Player.Navigate(new GameOverview(_actieve_gebruiker, Frame_Player, null, _geselecteerde_gebruiker.Player));
            }
        }

        private void BT_Player_Statistieken_Click(object sender, RoutedEventArgs e)
        {

        }

        
    }
}
