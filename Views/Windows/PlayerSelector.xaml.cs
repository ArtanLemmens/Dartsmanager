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
using System.Windows.Shapes;

namespace Dartsmanager.Views.Windows
{
    /// <summary>
    /// Interaction logic for PlayerSelector.xaml
    /// </summary>
    public partial class PlayerSelector : Window
    {
        public Player? _geselecteerde_speler = null;
        public PlayerSelector()
        {
            InitializeComponent();
            BindData();
        }

        private void BindData()
        {
            List<Player> spelers = PlayerService.GetAll();
            CB_Spelers.ItemsSource = spelers;
        }
        private void BT_Select_Click(object sender, RoutedEventArgs e)
        {
            if (CB_Spelers.SelectedItem is Player speler)
            {
                _geselecteerde_speler = speler;
                Close();
            }
            else
            {
                MessageBox.Show("Gelieve een speler te selecteren");
            }
        }

        private void BT_Create_Speler_Click(object sender, RoutedEventArgs e)
        {
            // Toon spelerscherm 
            var SpelerScherm = new PlayerWindow();
            SpelerScherm.ShowDialog();
            BindData();
        }

        private void BT_Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
