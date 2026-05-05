using Dartsmanager.Models;
using Dartsmanager.Services;
using Dartsmanager.Views.Windows;
using System;
using System.Collections.Generic;
using System.IO.Packaging;
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
        private Frame _frame;
        private Tournament? _actief_tornooi = null;

        public PlayerOverview(User? actieve_gebruiker, Frame frame, Tournament? actief_tornooi = null)
        {
            InitializeComponent();
            _actieve_gebruiker = actieve_gebruiker;
            _frame = frame;
            _actief_tornooi = actief_tornooi;
            GetPlayers();
        }

        private void GetPlayers()
        {
            List<Player> spelers;
            spelers = PlayerService.GetAll(_actief_tornooi);
            LB_Spelers.ItemsSource = spelers;
            LB_Spelers.Items.Refresh();
        }

        private void FilterPlayers()
        {
            if (LB_Spelers == null)
            {
                return;
            }
            string filter = TB_Speler.Text;
            List<Player> spelers;
            // Bij een lege waarde of "zoek speler" mogen al de spelers getoond worden
            if (string.IsNullOrWhiteSpace(filter) || filter == "Zoek speler...")
            {
                spelers = PlayerService.GetAll(_actief_tornooi);
            }
            else
            {
                // filteren op de gefilterde waarde
                spelers = PlayerService.GetPlayersFromNameFilter(filter, _actief_tornooi);
            }            
            LB_Spelers.ItemsSource = spelers;
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
            if (_actieve_gebruiker == null)
            {
                MessageBox.Show("Login in om de speler te kunnen aanmaken");
                return;
            }
            // Toon spelerscherm 
            var SpelerScherm = new PlayerWindow();
            SpelerScherm.ShowDialog();
            FilterPlayers();
        }

        private void BT_View_Player_Click(object sender, RoutedEventArgs e)
        {
            if(LB_Spelers.SelectedItem is Player speler)
            {
                _frame.Navigate(new PlayerPage(_actieve_gebruiker,speler,_frame));
            }
            else
            {
                MessageBox.Show("Gelieve eerst een speler te selecteren");
            }
        }

        private void BT_Remove_Player_Click(object sender, RoutedEventArgs e)
        {
            
            if (LB_Spelers.SelectedItem is Player speler)
            {
                if (_actieve_gebruiker == null)
                {
                    MessageBox.Show("Login in om de speler te kunnen verwijderen");
                }
                // Enkel verwijderen toelaten als de speler bij de actieve gebruiker hoort of als hij een admin is
                else if (_actieve_gebruiker.IsAdmin == true || (_actieve_gebruiker.PlayerId == speler.Id && _actieve_gebruiker.PlayerIdBevestigd == true))
                {
                    MessageBoxResult result = MessageBox.Show($"Bent u zeker dat u deze speler wenst te verwijderen:\n{speler.VoornaamNaam}?",
                                                  "Bevestig verwijdering",
                                                  MessageBoxButton.YesNo,
                                                  MessageBoxImage.Question);
                    if (result == MessageBoxResult.Yes)
                    {
                        try
                        {
                            PlayerService.Remove(speler);
                            FilterPlayers();
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
                MessageBox.Show("Gelieve eerst een speler te selecteren");
            }
        }

        
    }
}
