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

            List<Status> statuses = TournamentService.GetAllStatus();
            CB_Statuses.ItemsSource = statuses;
        }

        private void LoadTournamentData()
        {
            if (_actief_tornooi != null)
            {
                TB_Naam.Text = _actief_tornooi.Naam;
                TB_Jaargang.Text = _actief_tornooi.Jaargang.ToString();
                CB_Adresses.SelectedValue = _actief_tornooi.AdresId;
                TB_Datum.Text = _actief_tornooi.Datum;
                TB_Max_Inschrijvingen.Text = _actief_tornooi.MaxInschrijvingen.ToString();
                if (_actief_tornooi.Status != null)
                {
                    CB_Statuses.SelectedValue = _actief_tornooi.StatusId;
                }
                TB_Ronde.Text = _actief_tornooi.ActieveRonde.ToString();
                // Beschikbaarheid tot aanpassen enkel voor een admin
                if (_actieve_gebruiker != null && _actieve_gebruiker.IsAdmin == true)
                {
                    BT_Update_Tornooi.Visibility = Visibility.Visible;
                    TB_Naam.IsReadOnly = false;
                    TB_Jaargang.IsReadOnly = false;
                    CB_Adresses.IsEnabled = true;
                    TB_Datum.IsReadOnly = false;
                    TB_Max_Inschrijvingen.IsReadOnly = false;
                    CB_Statuses.IsEnabled = true;
                    BT_Create_Adress.Visibility = Visibility.Visible;                    
                }
                else
                {
                    BT_Update_Tornooi.Visibility = Visibility.Collapsed;
                    TB_Naam.IsReadOnly = true;
                    TB_Jaargang.IsReadOnly = true;
                    CB_Adresses.IsEnabled = false;
                    TB_Datum.IsReadOnly = true;
                    TB_Max_Inschrijvingen.IsReadOnly = true;
                    CB_Statuses.IsEnabled = false;
                    BT_Create_Adress.Visibility = Visibility.Collapsed;
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

                // Status ophalen
                if (CB_Statuses.SelectedItem is Status status)
                {
                    _actief_tornooi.StatusId = status.Id;
                }

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
    }
}
