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
    /// Interaction logic for TournamentWindow.xaml
    /// </summary>
    public partial class TournamentWindow : Window
    {
        public TournamentWindow()
        {
            InitializeComponent();
            BindData();
            TB_Naam.Focus();
        }

        private void BindData()
        {
            List<Adress> adressen = AdressService.GetAll();
            adressen.Add(new Adress { Straat = "-- Selecteer adres --" });
            CB_Adresses.ItemsSource = adressen;

            List <Tournament> tornooien =  TournamentService.GetAll();
            List<string> tornooiNamen = new List<string>();
            foreach(Tournament tornooi in tornooien)
            {
                if (!tornooiNamen.Contains(tornooi.Naam))
                {
                    tornooiNamen.Add(tornooi.Naam);
                }
            }
            tornooiNamen.Sort();
            tornooiNamen.Add("-- Nieuwe naam --");
            CB_Namen.ItemsSource = tornooiNamen;
        }

        private void CB_Namen_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CB_Namen.SelectedValue is string &&  (string)CB_Namen.SelectedValue == "-- Nieuwe naam --")
            {
                TB_Naam.Visibility = Visibility.Visible;
                TB_Naam.Focus();
            }
            else
            {
                TB_Naam.Visibility = Visibility.Collapsed;
            }
        }

        private void BT_Create_Adress_Click(object sender, RoutedEventArgs e)
        {
            var AdresScherm = new AdressWindow();
            AdresScherm.ShowDialog();
            BindData();
        }

        private void BT_Create_Click(object sender, RoutedEventArgs e)
        {
            // Gegevens ophalen
            // Naam ophalen uit combo of textbox (indien nieuwe)
            string naam = "";
            if (CB_Namen.SelectedValue is string)
            {
                naam = (string)CB_Namen.SelectedValue;
            }
            else
            {
                return;
            }
            if((string)CB_Namen.SelectedValue == "-- Nieuwe naam --")
            {
                naam = TB_Naam.Text;
            }

            // Jaargang ophalen en kijken of het tornooi nog niet bestaat in deze jaargang
            if (!int.TryParse(TB_Jaargang.Text, out int jaargang))
            {
                MessageBox.Show("Graag een correcte jaargang."); return;
            }
            if (TournamentService.CheckExistingJaargang(naam, jaargang) == true)
            {
                MessageBox.Show("Er bestaat al een tornooi in deze jaargang."); return;
            }

            // Adres ophalen
            int? adresId = null;
            if (CB_Adresses.SelectedItem is Adress adres)
            {
                adresId = adres.Id;
            }

            // Datum ophalen
            string datum = TB_Datum.Text;

            // Max Inschrijving ophalen en kijken of het deelbaar is door een macht van 2 & groter dan 3
            if (!int.TryParse(TB_MaxInschrijving.Text, out int max_inschrijvingen))
            {
                MessageBox.Show("Graag een correct aantal max inschrijvingen."); return;
            }
            if (TournamentService.CheckMaxInschrijvingen(max_inschrijvingen) == false)
            {
                MessageBox.Show("Het aantal inschrijving moet minstens 4 en een macht van 2 zijn, anders komen de rondes niet uit."); return;
            }

            // Tornooi aanmaken
            Tournament nieuw_tornooi = new Tournament
            {
                Naam = naam,
                Jaargang = jaargang,
                AdresId = adresId,
                Datum = datum,
                MaxInschrijvingen = max_inschrijvingen
            };

            TournamentService.Add(nieuw_tornooi);

            Close();
        }

        private void BT_Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        
    }
}
