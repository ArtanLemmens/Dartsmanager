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
    /// Interaction logic for AdressPage.xaml
    /// </summary>
    public partial class AdressPage : Page
    {

        private User? _actieve_gebruiker = null;
        private Adress? _actief_adres = null;
        private Frame _frame;

        public AdressPage(User? actieve_gebruiker, Adress adres, Frame frame)
        {
            InitializeComponent();
            _actieve_gebruiker = actieve_gebruiker;
            _actief_adres = adres;
            _frame = frame;
            LoadAdressData();
            BindData();
        }
        private void BindData()
        {
            List<Country> landen = AdressService.GetAllCountry();
            landen.Add(new Country { Naam = "-- Selecteer land --" });
            CB_Landen.ItemsSource = landen;
        }
        private void LoadAdressData()
        {
            if (_actief_adres != null)
            {
                TB_Straat.Text = _actief_adres.Straat;
                TB_Huisnummer.Text = _actief_adres.Huisnummer.ToString();
                TB_Toevoeging.Text = _actief_adres.Toevoeging;
                TB_Postcode.Text = _actief_adres.Postcode;
                TB_Gemeente.Text = _actief_adres.Gemeente;
                CB_Landen.SelectedValue = _actief_adres.CountryId;
            }
        }
        private void BT_AdressOverview_Click(object sender, RoutedEventArgs e)
        {
            _frame.Navigate(new AdressOverview(_actieve_gebruiker, _frame));
        }

        private void BT_Update_Adress_Click(object sender, RoutedEventArgs e)
        {
            if (_actieve_gebruiker != null && _actief_adres != null && _actieve_gebruiker.IsAdmin == true)
            {
                _actief_adres.Straat = TB_Straat.Text;
                if (!int.TryParse(TB_Huisnummer.Text, out int huisnummer))
                {
                    MessageBox.Show("Graag een geldig huisnummer opgeven!"); return;
                }
                _actief_adres.Huisnummer = huisnummer;

                _actief_adres.Toevoeging = TB_Toevoeging.Text;
                _actief_adres.Postcode = TB_Postcode.Text;
                _actief_adres.Gemeente = TB_Gemeente.Text;

                if (CB_Landen.SelectedItem is Country land)
                {
                    _actief_adres.CountryId = land.Id;
                }
                AdressService.Update(_actief_adres);
                MessageBox.Show("De adresgegevens zijn aangepast");
            }
            else
            {
                MessageBox.Show("U heeft geen rechten om deze adresgegevens te wijzigen");
            }
        }

        private void BT_Create_Country_Click(object sender, RoutedEventArgs e)
        {
            var LandScherm = new CountryWindow();
            LandScherm.ShowDialog();
            BindData();
        }

    }
}
