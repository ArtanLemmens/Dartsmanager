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
    /// Interaction logic for HomePage.xaml
    /// </summary>
    public partial class HomePage : Page
    {
        private User? _actieve_gebruiker = null;
        private Frame _frame;

        public HomePage(User? actieve_gebruiker, Frame frame)
        {
            InitializeComponent();
            _actieve_gebruiker = actieve_gebruiker;
            _frame = frame;
            GetTournaments();
            BindData();
        }

        private void BindData()
        {
            var resultaat = PlayerService.GetHighScores();
            if(resultaat.speler_180 != null)
            {
                TB_Meeste180.Text = $"{resultaat.speler_180.VoornaamNaam}: {resultaat.punten_180}";
            }
            if (resultaat.speler_gemiddelde != null)
            {
                TB_HoogsteGemiddelde.Text = $"{resultaat.speler_gemiddelde.VoornaamNaam}: {resultaat.punten_gemiddelde}";
            }
        }

        private void GetTournaments()
        {
            List<Tournament> actieve_tornooien;
            actieve_tornooien = TournamentService.GetAllOpen();
            LB_Tornooien.ItemsSource = actieve_tornooien;
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
    }
}
