using Dartsmanager.Data;
using Dartsmanager.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dartsmanager.Services
{
    public class UserService
    {
        public static List<User> GetAll()
        {
            using (var db = new DbDartsmanagerContext())
            {
                var gebruikers = db.Users.Include(u => u.Player).ToList();
                return gebruikers;
            }
        }
        public static User? GetUserFromId(int id)
        {
            using (var db = new DbDartsmanagerContext())
            {
                var gebruiker = db.Users.FirstOrDefault(u => u.Id == id);
                return gebruiker;
            }
        }
        public static User? GetUserFromName(string naam)
        {
            using (var db = new DbDartsmanagerContext())
            {
                var gebruiker = db.Users.FirstOrDefault(u => u.Username == naam);
                return gebruiker;
            }
        }
        public static bool CheckExistingName(string naam)
        {
            using (var db = new DbDartsmanagerContext())
            {
                bool bestaande_naam = false;
                var gebruiker = db.Users.FirstOrDefault(u => u.Username == naam);
                if (gebruiker != null)
                {
                    bestaande_naam = true;
                }
                return bestaande_naam;
            }
        }
        public static bool CheckExistingName(string naam, int id)
        {
            using (var db = new DbDartsmanagerContext())
            {
                bool bestaande_naam = false;
                var gebruiker = db.Users.FirstOrDefault(u => u.Username == naam && u.Id != id);
                if (gebruiker != null)
                {
                    bestaande_naam = true;
                }
                return bestaande_naam;
            }
        }
        public static bool CheckExistingPlayerLink(Player speler)
        {
            using (var db = new DbDartsmanagerContext())
            {
                bool bestaande_link = false;
                var gebruiker = db.Users.FirstOrDefault(u => u.PlayerId == speler.Id);
                if (gebruiker != null)
                {
                    bestaande_link = true;
                }
                return bestaande_link;
            }
        }
        public static bool CheckExistingPlayerLink(Player speler, int id)
        {
            using (var db = new DbDartsmanagerContext())
            {
                bool bestaande_link = false;
                var gebruiker = db.Users.FirstOrDefault(u => u.PlayerId == speler.Id && u.Id != id);
                if (gebruiker != null)
                {
                    bestaande_link = true;
                }
                return bestaande_link;
            }
        }

        public static int CountAdmins()
        {
            using (var db = new DbDartsmanagerContext())
            {
                int aantal_admins = db.Users.Count(u => u.IsAdmin);
                return aantal_admins;
            }
        }
        public static void Update(User gebruiker)
        {
            try
            {
                using (var db = new DbDartsmanagerContext())
                {
                    var bestaandeGebruiker = db.Users.FirstOrDefault(u => u.Id == gebruiker.Id);
                    if (bestaandeGebruiker != null)
                    {
                        bestaandeGebruiker.Username = gebruiker.Username;
                        bestaandeGebruiker.WachtwoordHash = gebruiker.WachtwoordHash;
                        bestaandeGebruiker.IsAdmin = gebruiker.IsAdmin;
                        bestaandeGebruiker.PlayerId = gebruiker.PlayerId;
                        bestaandeGebruiker.PlayerIdBevestigd = gebruiker.PlayerIdBevestigd;
                        db.SaveChanges();
                    }
                }
            }
            catch
            {
                throw new InvalidOperationException("De wijzigingen konden niet worden opgeslagen. Controleer of de gebruiker nog wordt gebruikt in andere gegevens.");
            }
        }
        public static void Add(User gebruiker)
        {
            try
            {
                using (var db = new DbDartsmanagerContext())
                {
                    db.Users.Add(gebruiker);
                    db.SaveChanges();
                }
            }
            catch
            {
                throw new InvalidOperationException("Kon de gebruiker niet toevoegen. Controleer of de naam uniek is of de databaseverbinding juist is.");
            }
        }
        public static void Remove(User gebruiker)
        {
            try
            {
                using (var db = new DbDartsmanagerContext())
                {
                    var bestaandeGebruiker = db.Users.FirstOrDefault(u => u.Id == gebruiker.Id);
                    if (bestaandeGebruiker != null)
                    {
                        db.Users.Remove(bestaandeGebruiker);
                        db.SaveChanges();
                    }
                }
            }
            catch
            {
                throw new InvalidOperationException("Deze gebruiker kan niet worden verwijderd omdat hij nog gekoppeld is aan een project of andere gegevens.");
            }
        }
    }
}
