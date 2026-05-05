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
    /// Interaction logic for TournamentOverview.xaml
    /// </summary>
    public partial class TournamentOverview : Page
    {
        private User? _actieve_gebruiker = null;
        private Frame _frame;
        private Player? _actieve_speler = null;

        public TournamentOverview(User? actieve_gebruiker, Frame frame, Player? actieve_speler = null)
        {
            InitializeComponent();
            _actieve_gebruiker = actieve_gebruiker;
            _actieve_speler = actieve_speler;
            _frame = frame;
            FilterTournaments();
        }

        private void FilterTournaments()
        {
            if (LB_Tornooien == null)
            {
                return;
            }
            string filter = TB_Tornooi.Text;

            List<Tournament> tornooien;

            // Bij een lege waarde of "zoek tornooi" mogen al de tornooien getoond worden
            if (string.IsNullOrWhiteSpace(filter) || filter == "Zoek tornooi...")
            {
                if (_actieve_speler != null)
                {
                    tornooien = TournamentService.GetAll(_actieve_speler);
                }
                else
                {
                    tornooien = TournamentService.GetAll();
                }
            }
            else
            {
                // filteren op de gefilterde waarde
                if (_actieve_speler != null)
                {
                    tornooien = TournamentService.GetTournamentsFromNameFilter(filter, _actieve_speler);
                }
                else
                {
                    tornooien = TournamentService.GetTournamentsFromNameFilter(filter);
                }
            }
            LB_Tornooien.ItemsSource = tornooien;
            LB_Tornooien.Items.Refresh();
        }

        private void TB_Tornooi_GotFocus(object sender, RoutedEventArgs e)
        {
            if (TB_Tornooi.Text == "Zoek tornooi...")
            {
                TB_Tornooi.Text = "";
            }
        }

        private void TB_Tornooi_LostFocus(object sender, RoutedEventArgs e)
        {
            if (TB_Tornooi.Text == "")
            {
                TB_Tornooi.Text = "Zoek tornooi...";
            }
        }

        private void TB_Tornooi_TextChanged(object sender, TextChangedEventArgs e)
        {
            FilterTournaments();
        }

        private void BT_Create_Tournament_Click(object sender, RoutedEventArgs e)
        {
            if (_actieve_gebruiker == null || _actieve_gebruiker.IsAdmin == false)
            {
                MessageBox.Show("Log in als admin om een tornooi aan te maken"); return;
            }
            // Toon tornooischerm 
            var TornooiScherm = new TournamentWindow();
            TornooiScherm.ShowDialog();
            FilterTournaments();
        }

        private void BT_View_Tournament_Click(object sender, RoutedEventArgs e)
        {
            if (LB_Tornooien.SelectedItem is Tournament tornooi)
            {
                _frame.Navigate(new TournamentPage(_actieve_gebruiker, tornooi, _frame));
            }
            else
            {
                MessageBox.Show("Gelieve eerst een tornooi te selecteren");
            }
        }

        private void BT_Remove_Tournament_Click(object sender, RoutedEventArgs e)
        {
            if (LB_Tornooien.SelectedItem is Tournament tornooi)
            {
                if (_actieve_gebruiker == null)
                {
                    MessageBox.Show("Login in als admin om het tornooi te kunnen verwijderen");
                }
                // Enkel verwijderen toelaten als hij een admin is
                else if (_actieve_gebruiker.IsAdmin == true )
                {
                    MessageBoxResult result = MessageBox.Show($"Bent u zeker dat u dit tornooi wenst te verwijderen:\n{tornooi.Naam}, jaargang: {tornooi.Jaargang}?",
                                                  "Bevestig verwijdering",
                                                  MessageBoxButton.YesNo,
                                                  MessageBoxImage.Question);
                    if (result == MessageBoxResult.Yes)
                    {
                        try
                        {
                            TournamentService.Remove(tornooi);
                            FilterTournaments();
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message, "Fout", MessageBoxButton.OK, MessageBoxImage.Warning);
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("Gelieve eerst een tornooi te selecteren");
            }
        }
    }
}
