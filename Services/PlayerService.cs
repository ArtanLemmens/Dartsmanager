using Dartsmanager.Data;
using Dartsmanager.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dartsmanager.Services
{
    public class PlayerService
    {
        public static List<Player> GetAll()
        {
            using (var db = new DbDartsmanagerContext())
            {
                var spelers = db.Players
                    .Include(p => p.Adres)
                    .Include(p => p.User)
                    .ToList();

                return spelers;
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
                    db.Players.Add(speler);
                    db.SaveChanges();
                }
            }
            catch
            {
                throw new InvalidOperationException("Kon de speler niet toevoegen. Controleer of de naam uniek is of de databaseverbinding juist is.");
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
                        db.Players.Remove(bestaandeSpeler);
                        db.SaveChanges();
                    }
                }
            }
            catch
            {
                throw new InvalidOperationException("Deze speler kan niet worden verwijderd omdat het nog gekoppeld is aan een project of andere gegevens.");
            }
        }
    }
}
