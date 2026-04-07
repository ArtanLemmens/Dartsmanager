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
    /// Interaction logic for CountryWindow.xaml
    /// </summary>
    public partial class CountryWindow : Window
    {
        public CountryWindow()
        {
            InitializeComponent();
            TB_Naam.Focus();
        }

        private void BT_Create_Click(object sender, RoutedEventArgs e)
        {
            // gegevens ophalen
            string naam = TB_Naam.Text;
            string afkorting = TB_Afkorting.Text;
            
            // Kijken of het land uniek is
            if (AdressService.CheckExistingCountry(naam, afkorting))
            {
                MessageBox.Show("Dit land of deze afkorting bestaat al."); return;
            }

            // Land aanmaken
            Country nieuw_land = new Country
            {
                Naam = naam,
                Afkorting = afkorting
            };
            AdressService.AddCountry(nieuw_land);
            Close();
        }

        private void BT_Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
