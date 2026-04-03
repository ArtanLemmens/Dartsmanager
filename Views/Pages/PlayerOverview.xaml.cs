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
    /// Interaction logic for PlayerOverview.xaml
    /// </summary>
    public partial class PlayerOverview : Page
    {
        private User? _actieve_gebruiker = null;

        public PlayerOverview(User? actieve_gebruiker)
        {
            InitializeComponent();
            _actieve_gebruiker = actieve_gebruiker;
            GetPlayers();
        }

        private void GetPlayers()
        {
            var spelers = PlayerService.GetAll();
            if (spelers.Count > 0)
            {
                LB_Spelers.ItemsSource = spelers;
            }
        }

        private void FilterPlayers()
        {
            if (LB_Spelers == null)
            {
                return;
            }
            string filter = TB_Speler.Text;
            // Bij een lege waarde of "zoek speler" mogen al de spelers getoond worden
            if (string.IsNullOrWhiteSpace(filter) || filter == "Zoek speler...")
            {
                var spelers = PlayerService.GetAll();
                if (spelers.Count > 0)
                {
                    LB_Spelers.ItemsSource = spelers;
                }
                return;
            }
            // filteren op de gefilterde waarde
            var gefilterde_spelers = PlayerService.GetPlayersFromNameFilter(filter);
            LB_Spelers.ItemsSource = gefilterde_spelers;
            LB_Spelers.Items.Refresh();
        }

        private void TB_Speler_GotFocus(object sender, RoutedEventArgs e)
        {
            if (TB_Speler.Text == "Zoek speler...")
            {
                TB_Speler.Text = "";
            }
        }

        private void TB_Speler_LostFocus(object sender, RoutedEventArgs e)
        {
            if (TB_Speler.Text == "")
            {
                TB_Speler.Text = "Zoek speler...";
            }
        }
        private void TB_Speler_TextChanged(object sender, TextChangedEventArgs e)
        {
            FilterPlayers();            
        }

        private void BT_Create_Player_Click(object sender, RoutedEventArgs e)
        {
            // Toon spelerscherm 
            var SpelerScherm = new PlayerWindow();
            SpelerScherm.ShowDialog();
        }

        
    }
}
