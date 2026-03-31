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
using System.Windows.Navigation;
using System.Windows.Shapes;

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
        }

        private void SetActiveUserValues()
        {
            if (_actieve_gebruiker != null)
            {
                TB_NotLoggedIn.Visibility = Visibility.Collapsed;
                Grid_UserInfo.Visibility = Visibility.Visible;
                TB_Username.Text = _actieve_gebruiker.Username;
                CB_Spelers.SelectedValue = _actieve_gebruiker.PlayerId;
            }
            else
            {
                TB_NotLoggedIn.Visibility = Visibility.Visible;
                Grid_UserInfo.Visibility = Visibility.Collapsed;
            }
        }

        private void BT_Create_Player_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BT_Update_Name_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BT_Update_Wachtwoord_Click(object sender, RoutedEventArgs e)
        {

        }

        //private void BT_Update_Click(object sender, RoutedEventArgs e)
        //{
        //    // Geen update toelaten als er geen gebruiker gelinkt is
        //    if (_actieve_gebruiker == null)
        //    {
        //        MessageBox.Show("Er is geen gebruiker ingelogd om up te daten"); return;
        //    }
        //    // Wachtwoord nakijken
        //    string password = TB_Wachtwoord.Password;
        //    if (!BC.Verify(password, _actieve_gebruiker.WachtwoordHash))
        //    {
        //        MessageBox.Show("Ongeldig wachtwoord!"); return;
        //    }
        //    // Username ophalen en kijken of hij niet bestaat in een andere gebruiker
        //    string username = TB_Username.Text;
        //    if (UserService.CheckExistingName(username, _actieve_gebruiker.Id) == true)
        //    {
        //        MessageBox.Show("Deze username bestaat al.\nGelieve een andere naam te kiezen."); return;
        //    }

        //    // Speler ophalen uit combobox indien er 1 is geselcteerd
        //    if (CB_Spelers.SelectedItem is Player speler)
        //    {
        //        if (UserService.CheckExistingPlayerLink(speler, _actieve_gebruiker.Id) == true)
        //        {
        //            MessageBox.Show("Deze speler is al gelinkt aan een andere gebruiker!\nNeem contact op met onze admins indien u dit wilt laten wijzigen."); return;
        //        }
        //        _actieve_gebruiker.PlayerId = speler.Id;
        //        _actieve_gebruiker.PlayerIdBevestigd = false;
        //    }
        //    _actieve_gebruiker.Username = username;

        //    // Aanpassen in de database
        //    UserService.Update(_actieve_gebruiker);
        //    Close();
        //}
    }
}
