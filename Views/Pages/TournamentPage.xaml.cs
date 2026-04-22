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
    /// Interaction logic for TournamentPage.xaml
    /// </summary>
    public partial class TournamentPage : Page
    {
        private User? _actieve_gebruiker = null;
        private Tournament? _actief_tornooi = null;
        private Frame _frame;

        public TournamentPage(User? actieve_gebruiker, Tournament tornooi, Frame frame)
        {
            InitializeComponent();
            _actieve_gebruiker = actieve_gebruiker;
            _actief_tornooi = tornooi;
            _frame = frame;
            LoadTournamentData();
            BindData();
        }
        private void BindData()
        {
            List<Adress> adressen = AdressService.GetAll();
            adressen.Add(new Adress { Straat = "-- Selecteer adres --" });
            CB_Adresses.ItemsSource = adressen;
        }

        private void LoadTournamentData()
        {
            if (_actief_tornooi != null)
            {
                TB_Naam.Text = _actief_tornooi.Naam;
                TB_Jaargang.Text = _actief_tornooi.Jaargang.ToString();
                CB_Adresses.SelectedValue = _actief_tornooi.AdresId;
                TB_Datum.Text = _actief_tornooi.Datum;
                // Inschrijvingen ophalen en tellen
                List<Registration> inschrijvingen = TournamentService.GetAllRegistrations(_actief_tornooi);                
                if (inschrijvingen.Count() > 0)
                {
                    TB_Inschrijvingen.Text = inschrijvingen.Count().ToString();
                }
                else
                {
                    TB_Inschrijvingen.Text = "0";
                }
                TB_Max_Inschrijvingen.Text = _actief_tornooi.MaxInschrijvingen.ToString();
                if (_actief_tornooi.Status != null)
                {
                    TB_Status.Text = _actief_tornooi.Status.Naam;
                }
                TB_Ronde.Text = _actief_tornooi.ActieveRonde.ToString();
                // Playercontrol tonen indien actieve gebruiker is ingelogd
                if (_actieve_gebruiker != null)
                {
                    Grid_PlayerControl.Visibility = Visibility.Visible;
                }
                else
                {
                    Grid_PlayerControl.Visibility = Visibility.Collapsed;
                }
                // Beschikbaarheid tot aanpassen enkel voor een admin
                if (_actieve_gebruiker != null && _actieve_gebruiker.IsAdmin == true)
                {
                    BT_Update_Tornooi.Visibility = Visibility.Visible;
                    TB_Naam.IsReadOnly = false;
                    TB_Jaargang.IsReadOnly = false;
                    CB_Adresses.IsEnabled = true;
                    TB_Datum.IsReadOnly = false;
                    TB_Max_Inschrijvingen.IsReadOnly = false;
                    BT_Create_Adress.Visibility = Visibility.Visible;
                    // Menu voor admincontrol zichtbaar maken
                    Grid_AdminControl.Visibility = Visibility.Visible;
                }
                else
                {
                    BT_Update_Tornooi.Visibility = Visibility.Collapsed;
                    TB_Naam.IsReadOnly = true;
                    TB_Jaargang.IsReadOnly = true;
                    CB_Adresses.IsEnabled = false;
                    TB_Datum.IsReadOnly = true;
                    TB_Max_Inschrijvingen.IsReadOnly = true;
                    BT_Create_Adress.Visibility = Visibility.Collapsed;
                    // Menu voor admincontrol onzichtbaar maken
                    Grid_AdminControl.Visibility = Visibility.Collapsed;
                }
            }
        }

        private void BT_TournamentOverview_Click(object sender, RoutedEventArgs e)
        {
            _frame.Navigate(new TournamentOverview(_actieve_gebruiker, _frame));
        }
        private void BT_Update_Tornooi_Click(object sender, RoutedEventArgs e)
        {
            if (_actieve_gebruiker != null && _actieve_gebruiker.IsAdmin == true )
            {
                if (_actief_tornooi == null)
                {
                    MessageBox.Show("Er is geen actief tornooi te wijzigen.");
                    return;
                }

                // kijken of het tornooi nog niet gestart is
                if (_actief_tornooi.Status != null && _actief_tornooi.Status.Naam != "Niet gestart")
                {
                    MessageBox.Show("U kan enkel een tornooi dat nog niet gestart is updaten.");
                    return;
                }

                _actief_tornooi.Naam = TB_Naam.Text;

                // Jaargang ophalen en kijken of het tornooi nog niet bestaat in deze jaargang
                if (!int.TryParse(TB_Jaargang.Text, out int jaargang))
                {
                    MessageBox.Show("Graag een correcte jaargang."); return;
                }
                if (TournamentService.CheckExistingJaargang(TB_Naam.Text, jaargang, _actief_tornooi.Id) == true)
                {
                    MessageBox.Show("Er bestaat al een tornooi in deze jaargang."); return;
                }
                _actief_tornooi.Jaargang = jaargang;

                // Adres ophalen
                if (CB_Adresses.SelectedItem is Adress adres)
                {
                    _actief_tornooi.AdresId = adres.Id;
                }

                // Datum ophalen
                _actief_tornooi.Datum = TB_Datum.Text;

                // Max Inschrijving ophalen en kijken of het deelbaar is door een macht van 2 & groter dan 3
                if (!int.TryParse(TB_Max_Inschrijvingen.Text, out int max_inschrijvingen))
                {
                    MessageBox.Show("Graag een correct aantal max inschrijvingen."); return;
                }
                if (TournamentService.CheckMaxInschrijvingen(max_inschrijvingen) == false)
                {
                    MessageBox.Show("Het aantal inschrijving moet minstens 4 en een macht van 2 zijn, anders komen de rondes niet uit."); return;
                }
                _actief_tornooi.MaxInschrijvingen = max_inschrijvingen;

                TournamentService.Update(_actief_tornooi);
                MessageBox.Show("De tornooigegevens zijn aangepast");
            }
            else
            {
                MessageBox.Show("U heeft geen rechten om deze tornooigegevens te wijzigen");
            }
        }
        private void BT_Create_Adress_Click(object sender, RoutedEventArgs e)
        {
            var AdresScherm = new AdressWindow();
            AdresScherm.ShowDialog();
            BindData();
        }

        // PLAYERCONTROL
        private void BT_Inschrijven_Click(object sender, RoutedEventArgs e)
        {
            if (_actief_tornooi == null)
            {
                MessageBox.Show("Er is geen actief tornooi om in te schrijven.");
                return;
            }
            if (_actieve_gebruiker == null || _actieve_gebruiker.PlayerId == null || _actieve_gebruiker.PlayerIdBevestigd == false)
            {
                MessageBox.Show("Er is geen actieve bevestigde speler om in te schrijven.");
                return;
            }
            var speler = PlayerService.GetPlayerFromId((int)_actieve_gebruiker.PlayerId);
            if (speler != null)
            {
                TournamentService.RegisterPlayer(_actief_tornooi, speler);
                Frame_Tournament.Navigate(new PlayerOverview(_actieve_gebruiker, Frame_Tournament, _actief_tornooi));
            }
        }
        private void BT_Uitschrijven_Click(object sender, RoutedEventArgs e)
        {
            if (_actief_tornooi == null)
            {
                MessageBox.Show("Er is geen actief tornooi om in te schrijven.");
                return;
            }
            if (_actieve_gebruiker == null || _actieve_gebruiker.PlayerId == null || _actieve_gebruiker.PlayerIdBevestigd == false)
            {
                MessageBox.Show("Er is geen actieve bevestigde speler om uit te schrijven.");
                return;
            }
            var speler = PlayerService.GetPlayerFromId((int)_actieve_gebruiker.PlayerId);
            if (speler != null)
            {
                TournamentService.EndPlayerRegistration(_actief_tornooi, speler);
                Frame_Tournament.Navigate(new PlayerOverview(_actieve_gebruiker, Frame_Tournament, _actief_tornooi));
            }
        }

        // ADMINCONTROL
        private void BT_Speler_Inschrijven_Click(object sender, RoutedEventArgs e)
        {
            var SpelerSelectie = new PlayerSelector();
            SpelerSelectie.ShowDialog();
            if (SpelerSelectie._geselecteerde_speler != null)
            {
                if (_actief_tornooi == null)
                {
                    MessageBox.Show("Er is geen actief tornooi geselecteerd.");
                    return;
                }
                TournamentService.RegisterPlayer(_actief_tornooi, SpelerSelectie._geselecteerde_speler);
                Frame_Tournament.Navigate(new PlayerOverview(_actieve_gebruiker, Frame_Tournament, _actief_tornooi));
            }
        }
        private void BT_Speler_Uitschrijven_Click(object sender, RoutedEventArgs e)
        {
            var SpelerSelectie = new PlayerSelector(_actief_tornooi);
            SpelerSelectie.ShowDialog();
            if (SpelerSelectie._geselecteerde_speler != null)
            {
                if (_actief_tornooi == null)
                {
                    MessageBox.Show("Er is geen actief tornooi geselecteerd.");
                    return;
                }
                TournamentService.EndPlayerRegistration(_actief_tornooi, SpelerSelectie._geselecteerde_speler);
                Frame_Tournament.Navigate(new PlayerOverview(_actieve_gebruiker, Frame_Tournament, _actief_tornooi));
            }
        }
        private void BT_Tournament_Start_Click(object sender, RoutedEventArgs e)
        {
            if (_actief_tornooi != null)
            {
                var status = TournamentService.Start(_actief_tornooi);
                if (status != null)
                {
                    _actief_tornooi.Status = status;
                    _actief_tornooi.StatusId = status.Id;
                    LoadTournamentData();
                }                

            }
        }
        private void BT_Tournament_Open_Click(object sender, RoutedEventArgs e)
        {
            if (_actief_tornooi != null)
            {
                var status = TournamentService.Open(_actief_tornooi);
                if (status != null)
                {
                    _actief_tornooi.Status = status;
                    _actief_tornooi.StatusId = status.Id;
                    // Dummyspelers terug verwijderen uit tornooi
                    var inschrijvingen_dummy = TournamentService.GetAllDummyRegistrations(_actief_tornooi);
                    foreach(var inschrijving in inschrijvingen_dummy)
                    {
                        TournamentService.RemoveRegistration(inschrijving);
                    }
                    LoadTournamentData();
                }
            }
        }
        private void BT_Tournament_Annuleer_Click(object sender, RoutedEventArgs e)
        {
            // Enkel annuleren wanneer het tornooi nog niet gestart is
            if (_actief_tornooi != null && _actief_tornooi.Status != null && _actief_tornooi.Status.Naam == "Niet gestart")
            {
                // Tornooi verwijderen (registraties vezrdwijnen automatisch mee)
                TournamentService.Remove(_actief_tornooi);
                MessageBox.Show("Het tornooi is geannuleerd.");
            }
            else
            {
                MessageBox.Show("Dit tornooi kan niet langer geannuleerd worden");
            }
        }
        private void BT_Tournament_Wedstrijdschema_Click(object sender, RoutedEventArgs e)
        {
            if (_actief_tornooi == null)
            {
                MessageBox.Show("Er is geen actief tornooi geselecteerd.");
                return;
            }
            TournamentService.CreateGroups(_actief_tornooi);
            TournamentService.CreateWedstrijdSchema(_actief_tornooi);
        }

        // TORNOOI SUBMENU
        private void BT_Tournament_Spelers_Click(object sender, RoutedEventArgs e)
        {
            Frame_Tournament.Navigate(new PlayerOverview(_actieve_gebruiker, Frame_Tournament, _actief_tornooi));
        }
        private void BT_Tournament_Groepsfase_Click(object sender, RoutedEventArgs e)
        {
            if (_actief_tornooi != null)
            {
                //Frame_Tournament.Navigate(new PlayerPage(_actieve_gebruiker, speler, Frame_Player));
            }
        }

        private void BT_Tournament_KOfase_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BT_Tournament_Wedstrijden_Click(object sender, RoutedEventArgs e)
        {

        }

        
    }
}
