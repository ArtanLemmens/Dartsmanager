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
        }
        
    }
}
