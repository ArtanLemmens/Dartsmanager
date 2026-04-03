using Dartsmanager.Models;
using Dartsmanager.Services;
using Dartsmanager.Views.Pages;
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

namespace Dartsmanager.Views.Windows
{
    /// <summary>
    /// Interaction logic for PlayerWindow.xaml
    /// </summary>
    public partial class PlayerWindow : Window
    {
        public PlayerWindow()
        {
            InitializeComponent();
            BindData();
            TB_Naam.Focus();
        }

        private void BindData()
        {
            List<User> gebruikers = UserService.GetAll();
            gebruikers.Insert(0, new User { Id = 0, Username = "-- Selecteer gebruiker --" });
            CB_Users.ItemsSource = gebruikers;
        }

        private void BT_Create_User_Click(object sender, RoutedEventArgs e)
        {

        }


        private void BT_Create_Click(object sender, RoutedEventArgs e)
        {
            // Naam & voornaam ophalen
            string naam = TB_Naam.Text;
            string voornaam = TB_Voornaam.Text;
            if (naam == "" || voornaam == "")
            {
                MessageBox.Show("Gelieve een naam en voornaam op te geven."); return;
            }
            // Kijken of de naam combinatie uniek is
            if (PlayerService.CheckExistingName(naam, voornaam) == true)
            {
                MessageBox.Show("Deze combinatie van naam en voornaam bestaat al.\nGelieve een andere naam te kiezen."); return;
            }

            // Geboortedatum ophalen
            string geboortedatum = DP_Geboortedatum.Text;

            // Mail ophalen
            string mail = TB_Mail.Text;

            // Telefoonnummer ophalen
            string telefoonnummer = TB_Telefoonnummer.Text;

            // Speler aanmaken
            Player nieuwe_speler = new Player
            {
                Voornaam = voornaam,
                Naam = naam,
                Geboortedatum = geboortedatum,
                Mail = mail,
                Telefoonnummer = telefoonnummer
            };                    

            // User ophalen uit combobox indien er 1 is geselecteerd
            if (CB_Users.SelectedItem is User gebruiker)
            {
                // Kijken of user nog niet gelinkt is aan andere speler
                if (PlayerService.CheckExistingUserLink(gebruiker) == true)
                {
                    MessageBox.Show("Deze gebruiker is al gelinkt aan een andere speler!\nNeem contact op met onze admins indien u dit wilt laten wijzigen."); return;
                }
                // Speler toevoegen
                PlayerService.Add(nieuwe_speler);
                //Gebruiker koppelen
                var gebruiker_te_koppelen = UserService.GetUserFromId(gebruiker.Id);
                if (gebruiker_te_koppelen != null)
                {
                    // Speler opnieuw ophalen (om id te bekomen)
                    var speler_te_koppelen = PlayerService.GetPlayerFromName(naam, voornaam);
                    if (speler_te_koppelen != null)
                    {
                        gebruiker_te_koppelen.PlayerId = speler_te_koppelen.Id;
                        gebruiker_te_koppelen.PlayerIdBevestigd = false;
                        UserService.Update(gebruiker_te_koppelen);
                        MessageBox.Show($"Uw aanvraag tot spelerskoppeling werd ingediend bij onze admins.");
                    }
                    
                }
            }
            else
            {
                // Speler toevoegen
                PlayerService.Add(nieuwe_speler);
            }
            Close();
        }

        private void BT_Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
