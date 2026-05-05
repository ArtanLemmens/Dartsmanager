using Dartsmanager.Models;
using Dartsmanager.Services;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
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
    /// Interaction logic for KOStagePage.xaml
    /// </summary>
    public partial class KOStagePage : Page
    {
        private User? _actieve_gebruiker = null;
        private Tournament? _actief_tornooi = null;
        private Frame _frame;

        private Dictionary<int, DataGrid> _groepSpelerGrids = new(); // om bij te houden in welke groep we bezig zijn

        public KOStagePage(User? actieve_gebruiker, Tournament tornooi, Frame frame)
        {
            InitializeComponent();
            _actieve_gebruiker = actieve_gebruiker;
            _actief_tornooi = tornooi;
            _frame = frame;
            BindData();
        }
        private void BindData()
        {
            if (_actief_tornooi != null && _actief_tornooi.ActieveRonde != null)
            {
                MaakGridWedstrijd();
            }
        }

        private void MaakGridWedstrijd()
        {
            if (_actief_tornooi != null)
            {
                var wedstrijden = GameService.GetAllfromKOstage(_actief_tornooi);

                StackPanel_Wedstrijden.Children.Clear();

                Grid grid = new Grid();

                // Aantal rondes ophalen
                int rondes = 1;
                if (_actief_tornooi.AantalRondes != null && _actief_tornooi.AantalRondes > 1)
                {
                    rondes = (int)_actief_tornooi.AantalRondes;
                }
                if (rondes <= 1)
                {
                    MessageBox.Show("Er zijn minstens 2 rondes nodig om een KO fase aan te maken"); return;
                }
                // voor elke ronde, behalve ronde 1 & finale, een kolom maken links en rechts & 1 in het midden voor de finale
                // aantal rijen & aantal kolommenbepalen
                int aantal_kolommen = rondes - 1; // geen kolom nodig voor ronde 1 want dit is de groepsfase
                if (_actief_tornooi.MaxInschrijvingen == null)
                {
                    return;
                    
                }
                int aantal_rijen = (int)_actief_tornooi.MaxInschrijvingen / 2 + 4; // 4 extra rijen voor de titels (Hoofdtitel reeks A/B + ondertitel ronde)
                // Rijen en kolommen op grid plaatsen
                for (int i = 0; i < aantal_kolommen; i++)
                {
                    grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
                }
                for (int i = 0; i < aantal_rijen; i++)
                {
                    grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
                }

                // --- TITELS --- met reeks A & B
                TextBlock titelA = new TextBlock
                {
                    Text = $"A-Reeks",
                    Style = (Style)TryFindResource("TextBlock_H2")
                };
                TextBlock titelB = new TextBlock
                {
                    Text = $"B-Reeks",
                    Style = (Style)TryFindResource("TextBlock_H2")
                };
                Grid.SetRow(titelA, 0); // Rij in grid bepalen
                Grid.SetColumn(titelA, 0); // Kolom in grid bepalen
                
                Grid.SetRow(titelB, aantal_rijen / 2); // Rij in grid bepalen
                Grid.SetColumn(titelB, 0); // Kolom in grid bepalen

                grid.Children.Add(titelA); // Toevoegen aan grid
                grid.Children.Add(titelB); // Toevoegen aan grid                

                // Wedstrijden per ronde ophalen en zetten
                for (int i = 2; i <= rondes; i++)
                {
                    var wedstrijden_ronde = GameService.GetAll(_actief_tornooi, i);
                    if (wedstrijden_ronde != null && wedstrijden_ronde.Count > 0)
                    {
                        // --- ONDERTITELS --- met reeks A & B
                        string tekst_ronde = $"Ronde {i}";
                        if (wedstrijden_ronde.Count <= 2)
                        {
                            tekst_ronde = "Finale";
                        }
                        else if (wedstrijden_ronde.Count <= 4)
                        {
                            tekst_ronde = "Halve finale";
                        }
                        else if (wedstrijden_ronde.Count <= 8)
                        {
                            tekst_ronde = "Kwart finale";
                        }
                        else if (wedstrijden_ronde.Count <= 16)
                        {
                            tekst_ronde = "Achtste finale";
                        }
                        else if (wedstrijden_ronde.Count <= 32)
                        {
                            tekst_ronde = "Zestiende finale";
                        }
                        TextBlock ondertitelA = new TextBlock
                        {
                            Text = tekst_ronde,
                            Style = (Style)TryFindResource("TextBlock_H3")
                        };
                        TextBlock ondertitelB = new TextBlock
                        {
                            Text = tekst_ronde,
                            Style = (Style)TryFindResource("TextBlock_H3")
                        };
                        Grid.SetRow(ondertitelA, 1); // Rij in grid bepalen
                        Grid.SetColumn(ondertitelA, i - 2); // Kolom in grid bepalen

                        Grid.SetRow(ondertitelB, aantal_rijen / 2 + 1); // Rij in grid bepalen
                        Grid.SetColumn(ondertitelB, i - 2); // Kolom in grid bepalen

                        grid.Children.Add(ondertitelA); // Toevoegen aan grid
                        grid.Children.Add(ondertitelB); // Toevoegen aan grid

                        for (int j = 0; j < wedstrijden_ronde.Count; j++)
                        {
                            // Border maken met de wedstrijd
                            Game wedstrijd = wedstrijden_ronde[j];
                            Border box = new Border
                            {
                                MinWidth = 20,
                                MinHeight = 20,
                                HorizontalAlignment = HorizontalAlignment.Left,
                                VerticalAlignment = VerticalAlignment.Top,
                                BorderBrush = System.Windows.Media.Brushes.DarkGray,
                                BorderThickness = new Thickness(1),
                                Background = System.Windows.Media.Brushes.Gray,
                                CornerRadius = new CornerRadius(4),
                                Margin = new Thickness(5),
                                Padding = new Thickness(3)
                            };

                            // INNER GRID per wedstrijd
                            Grid innerGrid = new Grid();

                            innerGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(200)});
                            innerGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(50) });
                            innerGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(50) });
                            innerGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(50) });

                            innerGrid.RowDefinitions.Add(new RowDefinition()); // header
                            innerGrid.RowDefinitions.Add(new RowDefinition()); // speler 1
                            innerGrid.RowDefinitions.Add(new RowDefinition()); // speler 2

                            // HEADER
                            string[] headers = { "Spelers", "Legs", "180", "Gem." };
                            for (int h = 0; h < headers.Length; h++)
                            {
                                var titel = new TextBlock
                                {
                                    Text = headers[h],
                                    FontWeight = FontWeights.Bold,
                                    FontSize = 13,
                                    Foreground = System.Windows.Media.Brushes.Orange,                                    
                                    Margin = new Thickness(2)
                                };
                                Grid.SetRow(titel, 0);
                                Grid.SetColumn(titel, h);
                                innerGrid.Children.Add(titel);
                            }


                            // FUNCTIE om rij toe te voegen, zodat we deze niet dubbel moeten uitschrijven
                            void AddRow(int row, Player speler)
                            {
                                var naam = new TextBlock
                                {
                                    Text = speler.VoornaamNaam,
                                    Margin = new Thickness(2),
                                    Foreground = System.Windows.Media.Brushes.White,
                                    Tag = "Naam"
                                };

                                var legs = new TextBox
                                {
                                    Text = GameService.GetLegsFromGamescore(wedstrijd, speler).ToString(),
                                    HorizontalContentAlignment = HorizontalAlignment.Center,
                                    Margin = new Thickness(2),
                                    Foreground = System.Windows.Media.Brushes.White,
                                    Background = System.Windows.Media.Brushes.Transparent,
                                    Tag = new { wedstrijd, speler}
                                };
                                legs.LostFocus += Legs_LostFocus;
                                // Enkel laten wijzigen door admin en wanneer de ronde nog actief is
                                var status = TournamentService.GetStatusByName("Afgelopen");
                                if (_actieve_gebruiker == null
                                    || (_actieve_gebruiker != null && _actieve_gebruiker.IsAdmin == false)
                                    || (_actief_tornooi != null && _actief_tornooi.ActieveRonde > i)
                                    || (_actief_tornooi != null && _actief_tornooi.Status != null && status != null && _actief_tornooi.StatusId == status.Id))
                                {
                                    legs.IsReadOnly = true;
                                }

                                var aantal_180 = new TextBox
                                {
                                    Text = GameService.Get180sFromGamescore(wedstrijd, speler).ToString(),
                                    HorizontalContentAlignment = HorizontalAlignment.Center,
                                    Margin = new Thickness(2),
                                    Foreground = System.Windows.Media.Brushes.White,
                                    Background = System.Windows.Media.Brushes.Transparent,
                                    Tag = new { wedstrijd, speler }
                                };
                                aantal_180.TextChanged += Aantal_180_LostFocus;
                                // Enkel laten wijzigen door admin en wanneer de ronde nog actief is
                                if (_actieve_gebruiker == null
                                    || (_actieve_gebruiker != null && _actieve_gebruiker.IsAdmin == false)
                                    || (_actief_tornooi != null && _actief_tornooi.ActieveRonde > i)
                                    || (_actief_tornooi != null && _actief_tornooi.Status != null && status != null && _actief_tornooi.StatusId == status.Id))
                                {
                                    aantal_180.IsReadOnly = true;
                                }

                                var gem = new TextBox
                                {
                                    Text = GameService.GetGemiddeldeFromGamescore(wedstrijd, speler).ToString(),
                                    HorizontalContentAlignment = HorizontalAlignment.Center,
                                    Margin = new Thickness(2),
                                    Foreground = System.Windows.Media.Brushes.White,
                                    Background = System.Windows.Media.Brushes.Transparent,
                                    Tag = new { wedstrijd, speler }
                                };
                                gem.TextChanged += Gemiddelde_LostFocus;
                                // Enkel laten wijzigen door admin en wanneer de ronde nog actief is
                                if (_actieve_gebruiker == null
                                    || (_actieve_gebruiker != null && _actieve_gebruiker.IsAdmin == false)
                                    || (_actief_tornooi != null && _actief_tornooi.ActieveRonde > i)
                                    || (_actief_tornooi != null && _actief_tornooi.Status != null && status != null && _actief_tornooi.StatusId == status.Id))
                                {
                                    gem.IsReadOnly = true;
                                }

                                Grid.SetRow(naam, row);
                                Grid.SetRow(legs, row);
                                Grid.SetRow(aantal_180, row);
                                Grid.SetRow(gem, row);

                                Grid.SetColumn(naam, 0);
                                Grid.SetColumn(legs, 1);
                                Grid.SetColumn(aantal_180, 2);
                                Grid.SetColumn(gem, 3);

                                innerGrid.Children.Add(naam);
                                innerGrid.Children.Add(legs);
                                innerGrid.Children.Add(aantal_180);
                                innerGrid.Children.Add(gem);
                            }

                            AddRow(1, wedstrijd.Player1);
                            AddRow(2, wedstrijd.Player2);

                            box.Child = innerGrid;

                            // positie bepalen
                            // kolom bepalen
                            Grid.SetColumn(box, i - 2);
                            // rij bepalen
                            int offset_rij = 2;
                            if (j >= wedstrijden_ronde.Count / 2)
                            {
                                offset_rij = (aantal_rijen / 2) + 2 - wedstrijden_ronde.Count / 2;
                            }
                            Grid.SetRow(box, j + offset_rij);
                            grid.Children.Add(box);
                        }
                    }
                }

                // Grid toevoegen aan de stackpanel
                StackPanel_Wedstrijden.Children.Add(grid);
            }
        }

        private void Legs_LostFocus(object sender, RoutedEventArgs e)
        {
            if (sender != null && sender is TextBox textbox)
            {
                if (int.TryParse(textbox.Text, out int nieuwe_waarde))
                {
                    // Gamescore zoeken om up te daten
                    dynamic data = textbox.Tag;
                    Game wedstrijd = data.wedstrijd;
                    Player speler = data.speler;
                    var score = GameService.GetScore(wedstrijd, speler);
                    if (score != null)
                    {
                        score.LegsWon = nieuwe_waarde;
                        GameService.UpdateGamescore(score);
                    }
                }
                else
                {
                    MessageBox.Show("Ongeldige invoer");
                    textbox.Text = "0";
                    textbox.Focus();
                }
            }
        }
        private void Aantal_180_LostFocus(object sender, RoutedEventArgs e)
        {
            if (sender != null && sender is TextBox textbox)
            {
                if (int.TryParse(textbox.Text, out int nieuwe_waarde))
                {
                    // Gamescore zoeken om up te daten
                    dynamic data = textbox.Tag;
                    Game wedstrijd = data.wedstrijd;
                    Player speler = data.speler;
                    var score = GameService.GetScore(wedstrijd, speler);
                    if (score != null)
                    {
                        score.Aantal180 = nieuwe_waarde;
                        GameService.UpdateGamescore(score);
                    }
                }
                else
                {
                    MessageBox.Show("Ongeldige invoer");
                    textbox.Text = "0";
                    textbox.Focus();
                }
            }
        }
        private void Gemiddelde_LostFocus(object sender, RoutedEventArgs e)
        {
            if (sender != null && sender is TextBox textbox)
            {
                if (double.TryParse(textbox.Text, out double nieuwe_waarde))
                {
                    // Gamescore zoeken om up te daten
                    dynamic data = textbox.Tag;
                    Game wedstrijd = data.wedstrijd;
                    Player speler = data.speler;
                    var score = GameService.GetScore(wedstrijd, speler);
                    if (score != null)
                    {
                        score.Gemiddelde = nieuwe_waarde;
                        GameService.UpdateGamescore(score);
                    }
                }
                else
                {
                    MessageBox.Show("Ongeldige invoer");
                    textbox.Text = "0";
                    textbox.Focus();
                }
            }
        }

    }
}
