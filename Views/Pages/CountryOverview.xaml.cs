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
    /// Interaction logic for CountryOverview.xaml
    /// </summary>
    public partial class CountryOverview : Page
    {
        private User? _actieve_gebruiker = null;
        private Frame _frame;

        public CountryOverview(User? actieve_gebruiker, Frame frame)
        {
            InitializeComponent();
            _actieve_gebruiker = actieve_gebruiker;
            _frame = frame;
            GetLanden();
        }

        private void GetLanden()
        {
            List<Country> landen;
            landen = AdressService.GetAllCountry();
            LB_Landen.ItemsSource = landen;
            LB_Landen.Items.Refresh();
        }

        private void FilterLanden()
        {
            if (LB_Landen == null)
            {
                return;
            }
            string filter = TB_Land.Text;
            List<Country> landen;
            // Bij een lege waarde of "zoek adres" mogen al de landen getoond worden
            if (string.IsNullOrWhiteSpace(filter) || filter == "Zoek land...")
            {
                landen = AdressService.GetAllCountry();
            }
            else
            {
                // filteren op de gefilterde waarde
                landen = AdressService.GetCountriesFromNameFilter(filter);
            }
            LB_Landen.ItemsSource = landen;
            LB_Landen.Items.Refresh();
        }


        private void TB_Land_GotFocus(object sender, RoutedEventArgs e)
        {
            if (TB_Land.Text == "Zoek land...")
            {
                TB_Land.Text = "";
            }
        }

        private void TB_Land_LostFocus(object sender, RoutedEventArgs e)
        {
            if (TB_Land.Text == "")
            {
                TB_Land.Text = "Zoek land...";
            }
        }

        private void TB_Land_TextChanged(object sender, TextChangedEventArgs e)
        {
            FilterLanden();
        }

        private void BT_Create_Country_Click(object sender, RoutedEventArgs e)
        {
            if (_actieve_gebruiker == null)
            {
                MessageBox.Show("Login in om het land te kunnen aanmaken");
                return;
            }
            // Toon landenscherm 
            var LandScherm = new CountryWindow();
            LandScherm.ShowDialog();
            FilterLanden();
        }

        private void BT_Remove_Country_Click(object sender, RoutedEventArgs e)
        {
            if (LB_Landen.SelectedItem is Country land)
            {
                if (_actieve_gebruiker == null)
                {
                    MessageBox.Show("Login in om het land te kunnen verwijderen");
                }
                // Enkel verwijderen toelaten als de actieve gebruiker een admin is
                else if (_actieve_gebruiker.IsAdmin == true)
                {
                    MessageBoxResult result = MessageBox.Show($"Bent u zeker dat u dit land wenst te verwijderen:\n{land.Naam}?",
                                                  "Bevestig verwijdering",
                                                  MessageBoxButton.YesNo,
                                                  MessageBoxImage.Question);
                    if (result == MessageBoxResult.Yes)
                    {
                        try
                        {
                            AdressService.RemoveCountry(land);
                            FilterLanden();
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
                MessageBox.Show("Gelieve eerst een land te selecteren");
            }
        }
    }
}
