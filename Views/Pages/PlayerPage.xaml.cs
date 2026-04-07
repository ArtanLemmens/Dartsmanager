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

namespace Dartsmanager.Views.Pages
{
    /// <summary>
    /// Interaction logic for PlayerPage.xaml
    /// </summary>
    public partial class PlayerPage : Page
    {
        private User? _actieve_gebruiker = null;
        private Player? _actieve_speler = null;
        private Frame _frame;

        public PlayerPage(User? actieve_gebruiker, Player speler, Frame frame)
        {
            InitializeComponent();
            _actieve_gebruiker = actieve_gebruiker;
            _actieve_speler = speler;
            _frame = frame;
            LoadPlayerData();
        }

        private void LoadPlayerData()
        {
            if (_actieve_speler != null)
            {
                TB_Voornaam.Text = _actieve_speler.Voornaam;
                TB_Naam.Text = _actieve_speler.Naam;
                TB_LidSinds.Text = _actieve_speler.LidSinds;
                TB_Ranking.Text = _actieve_speler.Ranking.ToString();
                // Private data enkel aan de gebruiker zelf of de admin tonen
                if (_actieve_gebruiker != null && (_actieve_gebruiker.IsAdmin == true || ( _actieve_gebruiker.PlayerId == _actieve_speler.Id && _actieve_gebruiker.PlayerIdBevestigd == true)))
                {
                    TB_Geboortedatum.Text = _actieve_speler.Geboortedatum;
                    TB_Mail.Text = _actieve_speler.Mail;
                    TB_Telefoonnummer.Text = _actieve_speler.Telefoonnummer;
                    TB_Adres.Text = _actieve_speler.AdresVolledig;
                }                
            }
        }

        private void BT_PlayerOverview_Click(object sender, RoutedEventArgs e)
        {
            _frame.Navigate(new PlayerOverview(_actieve_gebruiker, _frame));
        }

        private void BT_Update_Speler_Click(object sender, RoutedEventArgs e)
        {
            if (_actieve_gebruiker != null && _actieve_speler != null && (_actieve_gebruiker.IsAdmin == true || (_actieve_gebruiker.PlayerId == _actieve_speler.Id && _actieve_gebruiker.PlayerIdBevestigd == true)))
            {
                _actieve_speler.Voornaam = TB_Voornaam.Text;
                _actieve_speler.Naam = TB_Naam.Text;
                _actieve_speler.Geboortedatum = TB_Geboortedatum.Text;
                _actieve_speler.Mail = TB_Mail.Text;
                _actieve_speler.Telefoonnummer = TB_Telefoonnummer.Text;
                PlayerService.Update(_actieve_speler);
                MessageBox.Show("De spelersgegevens zijn aangepast");
            }
            else
            {
                MessageBox.Show("U heeft geen rechten om deze spelersgegevens te wijzigen");
            }
        }
    }
}

