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
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Dartsmanager.Views.Pages
{
    /// <summary>
    /// Interaction logic for AdressOverview.xaml
    /// </summary>
    public partial class AdressOverview : Page
    {
        private User? _actieve_gebruiker = null;
        private Frame _frame;

        public AdressOverview(User? actieve_gebruiker, Frame frame)
        {
            InitializeComponent();
            _actieve_gebruiker = actieve_gebruiker;
            _frame = frame;
            GetAdresses();
        }
        private void GetAdresses()
        {
            List<Adress> adressen;
            adressen = AdressService.GetAll();
            LB_Adressen.ItemsSource = adressen;
            LB_Adressen.Items.Refresh();
        }

        private void FilterAdressen()
        {
            if (LB_Adressen == null)
            {
                return;
            }
            string filter = TB_Adres.Text;
            List<Adress> adressen;
            // Bij een lege waarde of "zoek adres" mogen al de adressen getoond worden
            if (string.IsNullOrWhiteSpace(filter) || filter == "Zoek adres...")
            {
                adressen = AdressService.GetAll();
            }
            else
            {
                // filteren op de gefilterde waarde
                adressen = AdressService.GetAdressesFromNameFilter(filter);
            }
            LB_Adressen.ItemsSource = adressen;
            LB_Adressen.Items.Refresh();
        }



        private void TB_Adres_GotFocus(object sender, RoutedEventArgs e)
        {
            if (TB_Adres.Text == "Zoek adres...")
            {
                TB_Adres.Text = "";
            }
        }

        private void TB_Adres_LostFocus(object sender, RoutedEventArgs e)
        {
            if (TB_Adres.Text == "")
            {
                TB_Adres.Text = "Zoek adres...";
            }
        }

        private void TB_Adres_TextChanged(object sender, TextChangedEventArgs e)
        {
            FilterAdressen();
        }


        private void BT_Create_Adress_Click(object sender, RoutedEventArgs e)
        {
            if (_actieve_gebruiker == null)
            {
                MessageBox.Show("Login in om de adres te kunnen aanmaken");
                return;
            }
            // Toon adresscherm 
            var AdresScherm = new AdressWindow();
            AdresScherm.ShowDialog();
            FilterAdressen();
        }

        private void BT_View_Adress_Click(object sender, RoutedEventArgs e)
        {
            if (LB_Adressen.SelectedItem is Adress adres)
            {
                _frame.Navigate(new AdressPage(_actieve_gebruiker, adres, _frame));
            }
            else
            {
                MessageBox.Show("Gelieve eerst een adres te selecteren");
            }
        }

        private void BT_Remove_Adress_Click(object sender, RoutedEventArgs e)
        {
            if (LB_Adressen.SelectedItem is Adress adres)
            {
                if (_actieve_gebruiker == null)
                {
                    MessageBox.Show("Login in om de speler te kunnen verwijderen");
                }
                // Enkel verwijderen toelaten als de actieve gebruiker een admin is
                else if (_actieve_gebruiker.IsAdmin == true)
                {
                    MessageBoxResult result = MessageBox.Show($"Bent u zeker dat u dit adres wenst te verwijderen:\n{adres.AdresVolledig}?",
                                                  "Bevestig verwijdering",
                                                  MessageBoxButton.YesNo,
                                                  MessageBoxImage.Question);
                    if (result == MessageBoxResult.Yes)
                    {
                        try
                        {
                            AdressService.Remove(adres);
                            FilterAdressen();
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
                MessageBox.Show("Gelieve eerst een adres te selecteren");
            }
        }
    }
}
