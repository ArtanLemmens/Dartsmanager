using Dartsmanager.Models;
using Dartsmanager.Services;
using System;
using System.Collections.Generic;
using System.Text;
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
    /// Interaction logic for GroupStagePage.xaml
    /// </summary>
    public partial class GroupStagePage : Page
    {
        private User? _actieve_gebruiker = null;
        private Tournament? _actief_tornooi = null;
        private Frame _frame;

        private Dictionary<int, DataGrid> _groepSpelerGrids = new(); // om bij te houden in welke groep we bezig zijn

        public GroupStagePage(User? actieve_gebruiker, Tournament tornooi, Frame frame)
        {
            InitializeComponent();
            _actieve_gebruiker = actieve_gebruiker;
            _actief_tornooi = tornooi;
            _frame = frame;
            BindData();
        }
        private void BindData()
        {
            if (_actief_tornooi != null)
            {
                var groepen = TournamentService.GetAllGroups(_actief_tornooi);
                LoadGroepen(groepen);
            }                
        }

        private void LoadGroepen(List<Group> groepen)
        {
            StackPanel_Groepen.Children.Clear();

            foreach (var groep in groepen)
            {
                // Dynamische grid per groep
                Grid grid = MaakGridGroep(groep);
                // Grid toevoegen aan de stackpanel
                StackPanel_Groepen.Children.Add(grid);
            }
        }

        private Grid MaakGridGroep(Group groep)
        {
            // Dynamisch methode om de groepen in grids aan te maken
            Grid grid = new Grid();
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto }); // titel
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto }); // tabel met spelersinfo & punten
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto }); // Togglebutton
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto }); // tabel met wedstrijden

            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

            // --- TITEL --- met groepsnummer
            TextBlock titel = new TextBlock
            {
                Text = $"Groep {groep.GroepNummer}",
                Style = (Style)TryFindResource("TextBlock_H3")
            };
            Grid.SetRow(titel, 0); // Rij in grid bepalen
            Grid.SetColumn(titel, 0); // Kolom in grid bepalen
            grid.Children.Add(titel); // Toevoegen aan grid

            // --- SPELERSINFO ---  binnen de groep ophalen en aanmaken
            var spelers = TournamentService.GetAllPlayersFromGroup(groep);
            List<GroupPlayerInfo> spelersinfo = new List<GroupPlayerInfo>();
            foreach( Player speler in spelers)
            {
                spelersinfo.Add(new GroupPlayerInfo
                {
                    Speler = speler,
                    GroepSets = TournamentService.GetSetsPerGroupsPlayer(groep, speler),
                    GroepLegs = TournamentService.GetLegsPerGroupsPlayer(groep, speler),
                    Groep180 = TournamentService.Get180PerGroupsPlayer(groep, speler),
                    GroepGemiddelde = TournamentService.GetGemiddeldePerGroupsPlayer(groep, speler),
                });
            }
            // Sorteren
            spelersinfo = spelersinfo
                .OrderByDescending(s => s.GroepSets)
                .ThenByDescending(s => s.GroepLegs)
                .ThenByDescending(s => s.Groep180)
                .ThenByDescending(s => s.GroepGemiddelde)
                .ToList();

            // --- DATAGRID spelersinfo --- per groep
            DataGrid dg_spelersinfo = new DataGrid
            {
                ItemsSource = spelersinfo,
                VerticalAlignment = VerticalAlignment.Top,
                Background = Brushes.Black,
                Foreground = Brushes.White,
                RowBackground = Brushes.DimGray,
                AlternatingRowBackground = Brushes.Gray,
                BorderThickness = new Thickness(0),
                FontSize = 14,
                CanUserResizeColumns = false,
                CanUserResizeRows = false,
                CanUserAddRows = false,
                CanUserSortColumns = false,
                IsReadOnly = true,
                SelectionMode = DataGridSelectionMode.Single,
                SelectionUnit = DataGridSelectionUnit.FullRow,
                HeadersVisibility = DataGridHeadersVisibility.Column,
                GridLinesVisibility = DataGridGridLinesVisibility.None,
                AutoGenerateColumns = false, // Zorgt voor vrije controle over de kolommen                
            };
            // Stijl voor de kolommen
            Style centerStyle = new Style(typeof(TextBlock));
            centerStyle.Setters.Add(new Setter(TextBlock.TextAlignmentProperty, TextAlignment.Center));
            // Kolommen bepalen in datagrid
            dg_spelersinfo.Columns.Add(new DataGridTextColumn
            {
                Header = "Spelers",
                MinWidth = 300,
                ElementStyle = centerStyle,
                Binding = new Binding("Speler.VoornaamNaam")
            });
            dg_spelersinfo.Columns.Add(new DataGridTextColumn
            {
                Header = "Sets",
                MinWidth = 100,
                ElementStyle = centerStyle,
                Binding = new Binding("GroepSets")
            });
            dg_spelersinfo.Columns.Add(new DataGridTextColumn
            {
                Header = "Legs",
                MinWidth = 100,
                ElementStyle = centerStyle,
                Binding = new Binding("GroepLegs")
            });
            dg_spelersinfo.Columns.Add(new DataGridTextColumn
            {
                Header = "180",
                MinWidth = 100,
                ElementStyle = centerStyle,
                Binding = new Binding("Groep180")
            }); dg_spelersinfo.Columns.Add(new DataGridTextColumn
            {
                Header = "Gemiddelde",
                MinWidth = 100,
                ElementStyle = centerStyle,
                Binding = new Binding("GroepGemiddelde")
            });
            // Stijl voor de headers
            dg_spelersinfo.ColumnHeaderStyle = new Style(typeof(DataGridColumnHeader))
            {
                Setters =
                {
                    new Setter(Control.BackgroundProperty, Brushes.DarkRed),
                    new Setter(Control.ForegroundProperty, Brushes.White),
                    new Setter(Control.FontWeightProperty, FontWeights.Bold),
                    new Setter(Control.HorizontalContentAlignmentProperty, HorizontalAlignment.Center),
                    new Setter(Control.PaddingProperty, new Thickness(8,4,8,4))
                }
            };
            Grid.SetRow(dg_spelersinfo, 1); // Rij in grid bepalen
            Grid.SetColumn(dg_spelersinfo, 0); // Kolom in grid bepalen
            grid.Children.Add(dg_spelersinfo); // Toevoegen aan grid

            // --- TOGGLEBUTTON---
            Border background = new Border
            {
                Background = Brushes.DarkOrange,
                BorderThickness = new Thickness(1),
                BorderBrush = Brushes.Black,
            };
            // Tekst bij togglebutton
            TextBlock txt = new TextBlock
            {
                Text = "Wedstrijdschema",
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Foreground = Brushes.White,
                Padding = new Thickness(8, 4, 8, 4),
                FontWeight = FontWeights.Bold,
                FontSize = 15,

            };
            ToggleButton tb = new ToggleButton
            {
                Style = (Style)FindResource("TB_PijlOB"),
                Margin = new Thickness(5),
                IsChecked = false,
                HorizontalAlignment = HorizontalAlignment.Right,
                VerticalAlignment = VerticalAlignment.Center,
            };
            Grid.SetRow(background, 2);// Rij in grid bepalen
            Grid.SetColumn(background, 0); // Kolom in grid bepalen
            Grid.SetRow(txt, 2); // Rij in grid bepalen
            Grid.SetColumn(txt, 0); // Kolom in grid bepalen
            Grid.SetRow(tb, 2); // Rij in grid bepalen
            Grid.SetColumn(tb, 0); // Kolom in grid bepalen
            grid.Children.Add(background); // Toevoegen aan grid
            grid.Children.Add(txt); // Toevoegen aan grid
            grid.Children.Add(tb); // Toevoegen aan grid


            // --- WEDSTRIJDSCHEMA ---  binnen de groep ophalen en aanmaken
            var wedstrijden = GameService.GetAll(groep);
            List<GameInfo> wedstrijdinfo = new List<GameInfo>();
            foreach (Game wedstrijd in wedstrijden)
            {
                wedstrijdinfo.Add(new GameInfo
                {
                    Wedstrijd = wedstrijd,
                    Speler1 = wedstrijd.Player1,
                    Speler2 = wedstrijd.Player2,
                    Legs1 = GameService.GetLegsFromGamescore(wedstrijd, wedstrijd.Player1),
                    Legs2 = GameService.GetLegsFromGamescore(wedstrijd, wedstrijd.Player2),
                    Number_180_1 = GameService.Get180sFromGamescore(wedstrijd, wedstrijd.Player1),
                    Number_180_2 = GameService.Get180sFromGamescore(wedstrijd, wedstrijd.Player2),
                    Gemiddelde1 = GameService.GetGemiddeldeFromGamescore(wedstrijd, wedstrijd.Player1),
                    Gemiddelde2 = GameService.GetGemiddeldeFromGamescore(wedstrijd, wedstrijd.Player2),
                });
            }

            // --- DATAGRID wedstrijdschema --- per groep
            DataGrid dg_wedstrijden = new DataGrid
            {
                ItemsSource = wedstrijdinfo,
                VerticalAlignment = VerticalAlignment.Top,
                Background = Brushes.Black,
                Foreground = Brushes.White,
                RowBackground = Brushes.DimGray,
                AlternatingRowBackground = Brushes.Gray,
                BorderThickness = new Thickness(0),
                FontSize = 14,
                CanUserResizeColumns = false,
                CanUserResizeRows = false,
                CanUserAddRows = false,
                CanUserSortColumns = false,
                IsReadOnly = false,
                SelectionMode = DataGridSelectionMode.Single,
                SelectionUnit = DataGridSelectionUnit.FullRow,
                HeadersVisibility = DataGridHeadersVisibility.Column,
                GridLinesVisibility = DataGridGridLinesVisibility.None,
                AutoGenerateColumns = false, // Zorgt voor vrije controle over de kolommen
            };
            // indien niet ingelogd alles read only zetten
            if (_actieve_gebruiker == null)
            {
                dg_wedstrijden.IsReadOnly = true;
            }
            // Zichtbaarheid vasthangen aan de togglebutton
            Binding binding = new Binding("IsChecked")
            {
                Source = tb,
                Converter = (IValueConverter)FindResource("BoolToVisibilityConverter")
            };
            dg_wedstrijden.SetBinding(VisibilityProperty, binding);

            // Kolommen bepalen in datagrid
            dg_wedstrijden.Columns.Add(new DataGridTextColumn
            {
                Header = "Speler 1",
                MinWidth = 200,
                ElementStyle = centerStyle,
                IsReadOnly = true,
                Binding = new Binding("Speler1.VoornaamNaam")
            });
            dg_wedstrijden.Columns.Add(new DataGridTextColumn
            {
                Header = "Legs",
                MinWidth = 50,
                ElementStyle = centerStyle,                
                Binding = new Binding("Legs1")
            });
            dg_wedstrijden.Columns.Add(new DataGridTextColumn
            {
                Header = "180",
                MinWidth = 50,
                ElementStyle = centerStyle,
                Binding = new Binding("Number_180_1")
            });
            dg_wedstrijden.Columns.Add(new DataGridTextColumn
            {
                Header = "Gem.",
                MinWidth = 50,
                ElementStyle = centerStyle,
                Binding = new Binding("Gemiddelde1")
            });

            dg_wedstrijden.Columns.Add(new DataGridTextColumn
            {
                Header = "Speler 2",
                MinWidth = 200,
                ElementStyle = centerStyle,
                IsReadOnly = true,
                Binding = new Binding("Speler2.VoornaamNaam")
            });
            dg_wedstrijden.Columns.Add(new DataGridTextColumn
            {
                Header = "Legs",
                MinWidth = 50,
                ElementStyle = centerStyle,
                Binding = new Binding("Legs2")
            });
            dg_wedstrijden.Columns.Add(new DataGridTextColumn
            {
                Header = "180",
                MinWidth = 50,
                ElementStyle = centerStyle,
                Binding = new Binding("Number_180_2")
            });
            dg_wedstrijden.Columns.Add(new DataGridTextColumn
            {
                Header = "Gem.",
                MinWidth = 50,
                ElementStyle = centerStyle,
                Binding = new Binding("Gemiddelde2")
            });
            // Methode voor het wijzigen van de scores up te daten in de database
            dg_wedstrijden.CellEditEnding += DgWedstrijden_CellEditEnding;

            // Stijl voor de headers
            dg_wedstrijden.ColumnHeaderStyle = new Style(typeof(DataGridColumnHeader))
            {
                Setters =
                {
                    new Setter(Control.BackgroundProperty, Brushes.DarkRed),
                    new Setter(Control.ForegroundProperty, Brushes.White),
                    new Setter(Control.FontWeightProperty, FontWeights.Bold),
                    new Setter(Control.HorizontalContentAlignmentProperty, HorizontalAlignment.Center),
                    new Setter(Control.PaddingProperty, new Thickness(8,4,8,4))
                }
            };
            Grid.SetRow(dg_wedstrijden, 3); // Rij in grid bepalen
            Grid.SetColumn(dg_wedstrijden, 0); // Kolom in grid bepalen
            grid.Children.Add(dg_wedstrijden); // Toevoegen aan grid

            // nummer toevoegen aan onze dictionary
            _groepSpelerGrids[groep.GroepNummer] = dg_spelersinfo;

            return grid;
        }

        private void DgWedstrijden_CellEditEnding(object? sender, DataGridCellEditEndingEventArgs e)
        {
            if (e.Row.Item is GameInfo gameInfo && e.EditingElement is TextBox nieuwe_legs_string)
            {
                // Kijken of nieuwe legs een int is
                int nieuwe_legs = -1;
                if (int.TryParse(nieuwe_legs_string.Text, out nieuwe_legs))
                {
                    if (nieuwe_legs >= 0 && nieuwe_legs <= 10)
                    {
                        // Kijken welke speler we moeten wijzigen                
                        Player? speler = null;
                        if (e.Column.Header?.ToString() == "Legs S1")
                        {
                            speler = gameInfo.Speler1;
                        }
                        else if (e.Column.Header?.ToString() == "Legs S2")
                        {
                            speler = gameInfo.Speler2;
                        }

                        
                        if (speler != null)
                        {
                            // kijken of de gebruiker wel rechten heeft
                            if (_actieve_gebruiker != null && (_actieve_gebruiker.IsAdmin == true || (_actieve_gebruiker.PlayerId == speler.Id && _actieve_gebruiker.PlayerIdBevestigd == true)))
                            {
                                // Update database
                                GameService.UpdateLegsFromGamescore(gameInfo.Wedstrijd, speler, nieuwe_legs);
                                GameService.UpdateSetFromLegs(gameInfo.Wedstrijd);
                            }
                            else
                            {
                                MessageBox.Show("U heeft geen rechten om deze score te wijzigen.");
                                // Oude waarde terugzetten
                                if (e.Column.Header?.ToString() == "Legs S1")
                                {
                                    nieuwe_legs_string.Text = gameInfo.Legs1.ToString();
                                }
                                else if (e.Column.Header?.ToString() == "Legs S2")
                                {
                                    nieuwe_legs_string.Text = gameInfo.Legs2.ToString();
                                }
                                return;
                            }
                        }

                        // Daarna tussenstand opnieuw berekenen
                        var groep = TournamentService.GetGroupFromGame(gameInfo.Wedstrijd);
                        if (groep != null)
                        {
                            RefreshGroep(groep);
                        }
                    }
                    else
                    {
                        MessageBox.Show("Voer een waarde in van 0 tem 10."); return;
                    }
                }
                else
                {
                    MessageBox.Show("Dit is geen geldige waarde!"); return;
                }                
            }
        }

        private void RefreshGroep(Group groep)
        {
            var spelers = TournamentService.GetAllPlayersFromGroup(groep);

            var nieuweLijst = spelers
                .Select(speler => new GroupPlayerInfo
                {
                    Speler = speler,
                    GroepSets = TournamentService.GetSetsPerGroupsPlayer(groep, speler),
                    GroepLegs = TournamentService.GetLegsPerGroupsPlayer(groep, speler),
                    Groep180 = TournamentService.Get180PerGroupsPlayer(groep, speler),
                    GroepGemiddelde = TournamentService.GetGemiddeldePerGroupsPlayer(groep, speler),
                })
                .OrderByDescending(s => s.GroepSets)
                .ThenByDescending(s => s.GroepLegs)
                .ThenByDescending(s => s.Groep180)
                .ThenByDescending(s => s.GroepGemiddelde)
                .ToList();

            if (_groepSpelerGrids.TryGetValue(groep.GroepNummer, out var grid))
            {
                grid.ItemsSource = nieuweLijst;
            }
        }

    }
}
