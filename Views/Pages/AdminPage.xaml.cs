using Dartsmanager.Models;
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
    /// Interaction logic for AdminPage.xaml
    /// </summary>
    public partial class AdminPage : Page
    {
        private User? _actieve_gebruiker = null;

        public AdminPage(User? actieve_gebruiker)
        {
            InitializeComponent();
            _actieve_gebruiker = actieve_gebruiker;
        }

        private void BT_Players_Click(object sender, RoutedEventArgs e)
        {
            Frame_Admin.Navigate(new PlayerOverview(_actieve_gebruiker, Frame_Admin));
        }

        private void BT_Tournaments_Click(object sender, RoutedEventArgs e)
        {
            Frame_Admin.Navigate(new TournamentOverview(_actieve_gebruiker, Frame_Admin));
        }

        private void BT_Users_Click(object sender, RoutedEventArgs e)
        {
            Frame_Admin.Navigate(new UserOverview(_actieve_gebruiker, Frame_Admin));
        }

        private void BT_Adresses_Click(object sender, RoutedEventArgs e)
        {
            Frame_Admin.Navigate(new AdressOverview(_actieve_gebruiker, Frame_Admin));
        }

        private void BT_Countries_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
