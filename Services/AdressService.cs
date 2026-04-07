using Dartsmanager.Data;
using Dartsmanager.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dartsmanager.Services
{
    public class AdressService
    {
        // Country
        public static List<Country> GetAllCountry()
        {
            using (var db = new DbDartsmanagerContext())
            {
                var landen = db.Countries.ToList();
                return landen;
            }
        }
        public static Country? GetCountryByName(string naam)
        {
            using (var db = new DbDartsmanagerContext())
            {
                var land = db.Countries.FirstOrDefault(c => c.Naam == naam);
                return land;
            }
        }

        public static bool CheckExistingCountry(string naam, string afkorting)
        {
            using (var db = new DbDartsmanagerContext())
            {
                bool bestaand_land = false;
                var land = db.Countries.FirstOrDefault(c => c.Naam == naam || c.Afkorting == afkorting);
                if (land != null)
                {
                    bestaand_land = true;
                }
                return bestaand_land;
            }
        }


        public static void UpdateCountry(Country land)
        {
            try
            {
                using (var db = new DbDartsmanagerContext())
                {
                    var bestaandLand = db.Countries.FirstOrDefault(c => c.Id == land.Id);
                    if (bestaandLand != null)
                    {
                        bestaandLand.Naam = land.Naam;
                        bestaandLand.Afkorting = land.Afkorting;
                        db.SaveChanges();
                    }
                }
            }
            catch
            {
                throw new InvalidOperationException("De wijzigingen konden niet worden opgeslagen. Controleer of het land nog wordt gebruikt in andere gegevens.");
            }
        }
        public static void AddCountry(Country land)
        {
            try
            {
                using (var db = new DbDartsmanagerContext())
                {
                    db.Countries.Add(land);
                    db.SaveChanges();
                }
            }
            catch
            {
                throw new InvalidOperationException("Kon het land niet toevoegen. Controleer of de naam uniek is of de databaseverbinding juist is.");
            }
        }
        public static void RemoveCountry(Country land)
        {
            try
            {
                using (var db = new DbDartsmanagerContext())
                {
                    var bestaandLand = db.Countries.FirstOrDefault(c => c.Id == land.Id);
                    if (bestaandLand != null)
                    {
                        db.Countries.Remove(bestaandLand);
                        db.SaveChanges();
                    }
                }
            }
            catch
            {
                throw new InvalidOperationException("Dit land kan niet worden verwijderd omdat het nog gekoppeld is aan een project of andere gegevens.");
            }
        }

        // Adress
        public static List<Adress> GetAll()
        {
            using (var db = new DbDartsmanagerContext())
            {
                var adressen = db.Adresses.Include(a => a.Country).ToList();
                return adressen;
            }
        }
        public static bool CheckExistingAdres(string straat, int huisnummer, string toevoeging, string postcode, string gemeente, int? countryId)
        {
            using (var db = new DbDartsmanagerContext())
            {
                bool bestaand_adres = false;
                var adres = db.Adresses.FirstOrDefault(a => a.Straat == straat
                && a.Huisnummer == huisnummer
                && a.Toevoeging == toevoeging
                && a.Postcode == postcode
                && a.Gemeente == gemeente
                && a.CountryId == countryId);
                if (adres != null)
                {
                    bestaand_adres = true;
                }
                return bestaand_adres;
            }
        }
        public static int? GetIdFromFullAdress(string straat, int huisnummer, string? toevoeging, string? postcode, string? gemeente, int? countryId)
        {
            using (var db = new DbDartsmanagerContext())
            {

                var adres = db.Adresses.FirstOrDefault(a => a.Straat == straat
                && a.Huisnummer == huisnummer
                && a.Toevoeging == toevoeging
                && a.Postcode == postcode
                && a.Gemeente == gemeente
                && a.CountryId == countryId);
                if (adres != null)
                {
                    return adres.Id;
                }
                return null;
            }
        }
        public static void Update(Adress adres)
        {
            try
            {
                using (var db = new DbDartsmanagerContext())
                {
                    var bestaandAdres = db.Adresses.FirstOrDefault(a => a.Id == adres.Id);
                    if (bestaandAdres != null)
                    {
                        bestaandAdres.Straat = adres.Straat;
                        bestaandAdres.Huisnummer = adres.Huisnummer;
                        bestaandAdres.Toevoeging = adres.Toevoeging;
                        bestaandAdres.Postcode = adres.Postcode;
                        bestaandAdres.Gemeente = adres.Gemeente;
                        bestaandAdres.CountryId = adres.CountryId;
                        db.SaveChanges();
                    }
                }
            }
            catch
            {
                throw new InvalidOperationException("De wijzigingen konden niet worden opgeslagen. Controleer of het adres nog wordt gebruikt in andere gegevens.");
            }
        }
        public static void Add(Adress adres)
        {
            try
            {
                using (var db = new DbDartsmanagerContext())
                {
                    db.Adresses.Add(adres);
                    db.SaveChanges();
                }
            }
            catch
            {
                throw new InvalidOperationException("Kon het adres niet toevoegen. Controleer of de naam uniek is of de databaseverbinding juist is.");
            }
        }
        public static void Remove(Adress adres)
        {
            try
            {
                using (var db = new DbDartsmanagerContext())
                {
                    var bestaandAdres = db.Adresses.FirstOrDefault(a => a.Id == adres.Id);
                    if (bestaandAdres != null)
                    {
                        db.Adresses.Remove(bestaandAdres);
                        db.SaveChanges();
                    }
                }
            }
            catch
            {
                throw new InvalidOperationException("Dit adres kan niet worden verwijderd omdat het nog gekoppeld is aan een project of andere gegevens.");
            }
        }
    }
}
