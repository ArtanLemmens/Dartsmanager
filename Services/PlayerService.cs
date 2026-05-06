using Dartsmanager.Data;
using Dartsmanager.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace Dartsmanager.Services
{
    public class PlayerService
    {
        public static List<Player> GetAll(Tournament? tornooi = null)
        {
            using (var db = new DbDartsmanagerContext())
            {
                if (tornooi == null)
                {
                    return db.Players
                        .Include(p => p.Adres)
                        .Include(p => p.User)
                        .Where(p => p.IsDummy == 0)
                        .OrderByDescending(p => p.RankingPoints)
                        .ToList();
                }
                else
                {
                    return db.Players
                    .Include(p => p.Adres)
                    .Include(p => p.User)
                    .Where(p => db.Registrations.Any(r => r.TournamentId == tornooi.Id && r.PlayerId == p.Id))
                    .OrderByDescending(p => p.RankingPoints)
                    .ToList();
                }
            }
        }
        public static Player? GetPlayerFromId(int id)
        {
            using (var db = new DbDartsmanagerContext())
            {
                var speler = db.Players.FirstOrDefault(p => p.Id == id);
                return speler;
            }
        }
        public static Player? GetPlayerFromName(string naam, string voornaam)
        {
            using (var db = new DbDartsmanagerContext())
            {
                var speler = db.Players.FirstOrDefault(p => p.Naam == naam && p.Voornaam == voornaam);
                return speler;
            }
        }

        public static List<Player> GetPlayersFromNameFilter(string filter, Tournament? tornooi = null)
        {
            using (var db = new DbDartsmanagerContext())
            {
                if (tornooi == null)
                {
                    return db.Players.Where(p => p.Voornaam.Contains(filter) || p.Naam.Contains(filter)).ToList();
                }
                else
                {
                    return db.Players
                    .Include(p => p.Adres)
                    .Include(p => p.User)
                    .Where(p => db.Registrations.Any(r => r.TournamentId == tornooi.Id && r.PlayerId == p.Id) && (p.Voornaam.Contains(filter) || p.Naam.Contains(filter)))
                    .ToList();
                }
            }
        }
        public static bool CheckExistingName(string naam, string voornaam)
        {
            using (var db = new DbDartsmanagerContext())
            {
                bool bestaande_naam = false;
                var speler = db.Players.FirstOrDefault(p => p.Naam == naam && p.Voornaam == voornaam);
                if (speler != null)
                {
                    bestaande_naam = true;
                }
                return bestaande_naam;
            }
        }
        public static bool CheckExistingName(string naam, string voornaam, int id)
        {
            using (var db = new DbDartsmanagerContext())
            {
                bool bestaande_naam = false;
                var speler = db.Players.FirstOrDefault(p => p.Naam == naam && p.Voornaam == voornaam && p.Id != id);
                if (speler != null)
                {
                    bestaande_naam = true;
                }
                return bestaande_naam;
            }
        }
        public static bool CheckExistingUserLink(User gebruiker)
        {
            using (var db = new DbDartsmanagerContext())
            {
                bool bestaande_link = false;
                var gevonden_gebruiker = db.Users.FirstOrDefault(u => u.Id == gebruiker.Id);
                if (gebruiker != null && gebruiker.PlayerId != null)
                {
                    bestaande_link = true;
                }
                return bestaande_link;
            }
        }
        public static void Update(Player speler)
        {
            try
            {
                using (var db = new DbDartsmanagerContext())
                {
                    var bestaandeSpeler= db.Players.FirstOrDefault(p => p.Id == speler.Id);
                    if (bestaandeSpeler != null)
                    {
                        bestaandeSpeler.Naam = speler.Naam;
                        bestaandeSpeler.Voornaam = speler.Voornaam;
                        bestaandeSpeler.IsDummy = speler.IsDummy;
                        bestaandeSpeler.Geboortedatum = speler.Geboortedatum;
                        bestaandeSpeler.Mail = speler.Mail;
                        bestaandeSpeler.AdresId = speler.AdresId;
                        bestaandeSpeler.Telefoonnummer = speler.Telefoonnummer;
                        bestaandeSpeler.Ranking = speler.Ranking;
                        bestaandeSpeler.RankingPoints = speler.RankingPoints;
                        bestaandeSpeler.LidSinds = speler.LidSinds;
                        db.SaveChanges();
                    }
                }
            }
            catch
            {
                throw new InvalidOperationException("De wijzigingen konden niet worden opgeslagen. Controleer of de speler nog wordt gebruikt in andere gegevens.");
            }
        }
        public static void Add(Player speler)
        {
            try
            {
                using (var db = new DbDartsmanagerContext())
                {
                    speler.LidSinds = DateTime.Now.ToString("dd/MM/yyyy");
                    db.Players.Add(speler);
                    db.SaveChanges();
                }
            }
            catch
            {
                throw new InvalidOperationException("Kon de speler niet toevoegen. Controleer of de naam uniek is of de databaseverbinding juist is.");
            }
        }
        public static void AddDummy(int dummy_nummer)
        {
            try
            {
                using (var db = new DbDartsmanagerContext())
                {
                    // Dummy speler aanmaken
                    Player nieuwe_speler = new Player
                    {
                        Voornaam = "Dummy",
                        Naam = $"{dummy_nummer}",
                        IsDummy = 1
                    };
                    // Kijken of deze al bestaat
                    if (CheckExistingName(nieuwe_speler.Naam, nieuwe_speler.Voornaam) == true)
                    {
                        return;
                    }
                    db.Players.Add(nieuwe_speler);
                    db.SaveChanges();
                }
            }
            catch
            {
                throw new InvalidOperationException("Kon de dummy  niet toevoegen. Controleer of de naam uniek is of de databaseverbinding juist is.");
            }
        }
        public static void Remove(Player speler)
        {
            try
            {
                using (var db = new DbDartsmanagerContext())
                {
                    var bestaandeSpeler = db.Players.FirstOrDefault(p => p.Id == speler.Id);
                    if (bestaandeSpeler != null)
                    {
                        
                        var bestaandeUser = db.Users.FirstOrDefault(u => u.PlayerId == speler.Id);
                        if (bestaandeUser  != null)
                        {
                            MessageBoxResult result = MessageBox.Show($"Deze speler komt nog voor in user:\n{bestaandeUser.Username}.\nBent u echt zeker dat u wilt verwijderen?",
                                                  "Bevestig verwijdering",
                                                  MessageBoxButton.YesNo,
                                                  MessageBoxImage.Question);
                            if (result == MessageBoxResult.Yes)
                            {
                                bestaandeUser.PlayerId = null;
                                UserService.Update(bestaandeUser);
                            }
                            else
                            {
                                return;
                            }
                        }
                        db.Players.Remove(bestaandeSpeler);
                        db.SaveChanges();
                    }
                }
            }
            catch
            {
                throw new InvalidOperationException("Deze speler kan niet worden verwijderd omdat het nog gekoppeld is aan andere gegevens.");
            }
        }

        // Methodes om de punten en ranking te berekenen
        public static void CalculatePoints()
        {
            using (var db = new DbDartsmanagerContext())
            {
                // Al de punten terug op 0 zetten
                var spelers =  db.Players.ToList();
                foreach (var speler in spelers)
                {
                    speler.RankingPoints = 0;
                }
                // Al de tornooien ophalen en doorlopen
                var tornooien = TournamentService.GetAll();
                foreach (var tornooi in tornooien)
                {
                    // lijst maken om de tijdelijke ranking van het tornooi bij te houden
                    var ranking_lijst = new List<TempRanking>();

                    // elke inschrijving ophalen en doorlopen
                    var inschrijvingen = TournamentService.GetAllRegistrations(tornooi);
                    foreach (var inschrijving in inschrijvingen)
                    {
                        // Tornooistats ophalen
                        var resultaat = TournamentService.GetTournamentstatistics(tornooi, inschrijving.Player);
                        // Toevoegen aan rankinglijst
                        ranking_lijst.Add(new TempRanking
                        {
                            PlayerId = inschrijving.Player.Id,
                            Groepsfase = resultaat.groepsfase,
                            Gewonnen_Matchen = resultaat.wedstrijden_gewonnen,
                            Legs = resultaat.legs,
                            Aantal_180 = resultaat.aantal_180,
                            Gemiddelde = resultaat.gemiddelde
                        });
                    }

                    // Tijdelijk rankinglijst sorteren
                    var gesorteerd = ranking_lijst
                        .OrderBy(r => r.Groepsfase)
                        .ThenByDescending(r => r.Gewonnen_Matchen)
                        .ThenByDescending(r => r.Legs)
                        .ThenByDescending(r => r.Aantal_180)
                        .ThenByDescending(r => r.Gemiddelde)
                        .ToList();

                    // Kijken wat de max registraties waren en omgekeerd punten toekennen
                    int max = 0;
                    if (tornooi.MaxInschrijvingen != null)
                    {
                        max = (int)tornooi.MaxInschrijvingen;
                    }

                    for (int i = 0; i < gesorteerd.Count; i++)
                    {
                        int punten = Math.Max(0, max - i); // geen waarde lager dan 0 toekennen

                        var speler = spelers.FirstOrDefault(p => p.Id == gesorteerd[i].PlayerId);
                        if (speler != null && speler.IsDummy == 0) // Geen punten geven aan dummies
                        {
                            speler.RankingPoints += punten;
                            Update(speler);
                        }
                    }
                }
            }
        }
        public static (Player? speler_180, int punten_180, Player? speler_gemiddelde, double punten_gemiddelde) GetHighScores()
        {
            using (var db = new DbDartsmanagerContext())
            {
                // lijst maken om de ranking van de speler bij te houden
                var ranking_lijst = new List<TempRanking>();

                // Al de tornooien ophalen en doorlopen
                var tornooien = TournamentService.GetAll();
                foreach (var tornooi in tornooien)
                {
                    // elke inschrijving ophalen en doorlopen
                    var inschrijvingen = TournamentService.GetAllRegistrations(tornooi);
                    foreach (var inschrijving in inschrijvingen)
                    {
                        // Tornooistats ophalen
                        var resultaat = TournamentService.GetTournamentstatistics(tornooi, inschrijving.Player);
                        var gevonden_ranking = ranking_lijst.FirstOrDefault(p => p.PlayerId == inschrijving.Player.Id);
                        if (gevonden_ranking != null)
                        {
                            gevonden_ranking.Legs = resultaat.legs;
                            gevonden_ranking.Gemiddelde = (gevonden_ranking.Gemiddelde + resultaat.gemiddelde) / 2;
                        }
                        else
                        {
                            // Nieuwe waarde toevoegen aan rankinglijst
                            ranking_lijst.Add(new TempRanking
                            {
                                PlayerId = inschrijving.Player.Id,
                                Gewonnen_Matchen = resultaat.wedstrijden_gewonnen,
                                Legs = resultaat.legs,
                                Aantal_180 = resultaat.aantal_180,
                                Gemiddelde = resultaat.gemiddelde
                            });
                        }
                        
                    }
                }

                // Rankinglijsten sorteren
                var gesorteerd180 = ranking_lijst.OrderByDescending(r => r.Aantal_180).ToList();
                var gesorteerdgemiddelde = ranking_lijst.OrderByDescending(r => r.Gemiddelde).ToList();
                var speler_180 = GetPlayerFromId(gesorteerd180[0].PlayerId);
                var speler_Gemiddelde = GetPlayerFromId(gesorteerdgemiddelde[0].PlayerId);

                return (speler_180, gesorteerd180[0].Aantal_180, speler_Gemiddelde, gesorteerd180[0].Gemiddelde);
            }
        }
        public static void CalculateCompleteRanking()
        {
            using (var db = new DbDartsmanagerContext())
            {
                CalculatePoints();
                // De ranking terug op 0 zetten
                var spelers = db.Players.ToList();
                foreach (var speler in spelers)
                {
                    speler.Ranking = 0;
                }
                // Spelers sorteren op punten
                var gesorteerde_spelers = spelers.OrderByDescending(r => r.RankingPoints).ToList();
                for (int i = 0; i < gesorteerde_spelers.Count; i++)
                {
                    gesorteerde_spelers[i].Ranking = i + 1;
                    Update(gesorteerde_spelers[i]);
                }
            }
        }
    }
}
