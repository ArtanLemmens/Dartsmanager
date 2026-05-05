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
    /// Interaction logic for GamePage.xaml
    /// </summary>
    public partial class GamePage : Page
    {
        private User? _actieve_gebruiker = null;
        private Game? _actieve_wedstrijd = null;
        private Frame _frame;

        public GamePage(User? actieve_gebruiker, Game wedstrijd, Frame frame)
        {
            InitializeComponent();
            _actieve_gebruiker = actieve_gebruiker;
            _actieve_wedstrijd = wedstrijd;
            _frame = frame;
            LoadGameData();
            
        }

        private void LoadGameData()
        {
            if (_actieve_wedstrijd != null)
            {
                TB_Speler1_Naam.Text = _actieve_wedstrijd.Player1.VoornaamNaam;
                TB_Speler2_Naam.Text = _actieve_wedstrijd.Player2.VoornaamNaam;

                var gamescore_speler_1 = GameService.GetScore(_actieve_wedstrijd, _actieve_wedstrijd.Player1);
                var gamescore_speler_2 = GameService.GetScore(_actieve_wedstrijd, _actieve_wedstrijd.Player2);
                if (gamescore_speler_1 != null)
                {
                    TB_Speler1_Legs.Text = gamescore_speler_1.LegsWon.ToString();
                    TB_Speler1_180.Text = gamescore_speler_1.Aantal180.ToString();
                    TB_Speler1_Gemiddelde.Text = gamescore_speler_1.Gemiddelde.ToString();
                }
                if (gamescore_speler_2 != null)
                {
                    TB_Speler2_Legs.Text = gamescore_speler_2.LegsWon.ToString();
                    TB_Speler2_180.Text = gamescore_speler_2.Aantal180.ToString();
                    TB_Speler2_Gemiddelde.Text = gamescore_speler_2.Gemiddelde.ToString();
                }
            }
        }

        private void BT_GameOverview_Click(object sender, RoutedEventArgs e)
        {
            _frame.Navigate(new GameOverview(_actieve_gebruiker, _frame));
        }

    }
}
