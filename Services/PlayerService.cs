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
                        .ToList();
                }
                else
                {
                    return db.Players
                    .Include(p => p.Adres)
                    .Include(p => p.User)
                    .Where(p => db.Registrations.Any(r => r.TournamentId == tornooi.Id && r.PlayerId == p.Id))
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
    }
}
