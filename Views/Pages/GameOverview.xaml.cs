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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Dartsmanager.Views.Pages
{
    /// <summary>
    /// Interaction logic for GameOverview.xaml
    /// </summary>
    public partial class GameOverview : Page
    {
        private User? _actieve_gebruiker = null;
        private Frame _frame;
        private Tournament? _actief_tornooi = null;
        private Player? _actieve_speler = null;

        public GameOverview(User? actieve_gebruiker, Frame frame, Tournament? actief_tornooi = null, Player? actieve_speler = null)
        {
            InitializeComponent();
            _actieve_gebruiker = actieve_gebruiker;
            _frame = frame;
            _actief_tornooi = actief_tornooi;
            _actieve_speler = actieve_speler;
            FilterGames();
        }
        private void FilterGames()
        {
            if (LB_Wedstrijden == null)
            {
                return;
            }
            string filter = TB_Wedstrijd.Text;
            List<Game> wedstrijden;
            // Bij een lege waarde of "zoek wedstrijd" mogen al de wedstrijden getoond worden
            if (string.IsNullOrWhiteSpace(filter) || filter == "Zoek wedstrijd...")
            {
                if (_actief_tornooi != null)
                {
                    wedstrijden = GameService.GetAll(_actief_tornooi);
                }
                else if (_actieve_speler != null)
                {
                    wedstrijden = GameService.GetAll(_actieve_speler);
                }
                else
                {
                    wedstrijden = GameService.GetAll(_actief_tornooi);
                }
                
            }
            else
            {
                // filteren op de gefilterde waarde
                if (_actief_tornooi != null)
                {
                    wedstrijden = GameService.GetGamesFromNameFilter(filter, null, _actief_tornooi);
                }
                else if (_actieve_speler != null)
                {
                    wedstrijden = GameService.GetGamesFromNameFilter(filter, _actieve_speler, null);
                }
                else
                {
                    wedstrijden = GameService.GetGamesFromNameFilter(filter);
                }
            }
            LB_Wedstrijden.ItemsSource = wedstrijden;
            LB_Wedstrijden.Items.Refresh();
        }

        private void TB_Wedstrijd_GotFocus(object sender, RoutedEventArgs e)
        {
            if (TB_Wedstrijd.Text == "Zoek wedstrijd...")
            {
                TB_Wedstrijd.Text = "";
            }
        }

        private void TB_Wedstrijd_LostFocus(object sender, RoutedEventArgs e)
        {
            if (TB_Wedstrijd.Text == "")
            {
                TB_Wedstrijd.Text = "Zoek wedstrijd...";
            }
        }

        private void TB_Wedstrijd_TextChanged(object sender, TextChangedEventArgs e)
        {
            FilterGames();
        }

        private void BT_View_Game_Click(object sender, RoutedEventArgs e)
        {
            if (LB_Wedstrijden.SelectedItem is Game wedstrijd)
            {
                //_frame.Navigate(new GamePage(_actieve_gebruiker, wedstrijd, _frame));
            }
            else
            {
                MessageBox.Show("Gelieve eerst een wedstrijd te selecteren");
            }
        }
    }
}
