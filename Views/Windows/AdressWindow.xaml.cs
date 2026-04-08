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
    /// Interaction logic for AdressWindow.xaml
    /// </summary>
    public partial class AdressWindow : Window
    {
        public AdressWindow()
        {
            InitializeComponent();
            BindData();
            TB_Straat.Focus();
        }

        private void BindData()
        {
            List<Country> landen = AdressService.GetAllCountry();
            landen.Add(new Country {Naam = "-- Selecteer land --" });
            CB_Landen.ItemsSource = landen;
        }

        private void BT_Create_Land_Click(object sender, RoutedEventArgs e)
        {
            var LandScherm = new CountryWindow();
            LandScherm.ShowDialog();
            BindData();
        }

        private void BT_Create_Click(object sender, RoutedEventArgs e)
        {
            // gegevens ophalen
            string straat = TB_Straat.Text;
            string huisnummer_txt = TB_Huisnummer.Text;
            if (!int.TryParse(huisnummer_txt, out int huisnummer))
            {
                MessageBox.Show("Graag een nummeriek huisnummer."); return;
            }
            string toevoeging = TB_Toevoeging.Text;
            string postcode = TB_Postcode.Text;
            string gemeente = TB_Gemeente.Text;

            if (straat == "" || postcode == "" || gemeente == "")
            {
                MessageBox.Show("Gelieve een straat, postcode en gemeente op te geven."); return;
            }
            int? countryId = null;
            if (CB_Landen.SelectedItem is Country land)
            {
                Country? gezocht_land = AdressService.GetCountryByName(land.Naam);
                if (gezocht_land != null)
                {
                    countryId = gezocht_land.Id;
                }
            }
            // Kijken of de adres combinatie uniek is
            if (AdressService.CheckExistingAdres(straat, huisnummer, toevoeging, postcode, gemeente, countryId))
            {
                MessageBox.Show("Deze combinatie van adres bestaat al.\nGelieve een ander adres te kiezen."); return;
            }

            // Adres aanmaken
            Adress nieuw_adres = new Adress
            {
                Straat = straat,
                Huisnummer = huisnummer,
                Toevoeging = toevoeging,
                Postcode = postcode,
                Gemeente = gemeente,
                CountryId = countryId
            };
            AdressService.Add(nieuw_adres);
            Close();
        }
        

        private void BT_Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
