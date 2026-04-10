using Dartsmanager.Data;
using Dartsmanager.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Dartsmanager.Services
{
    public class TournamentService
    {
        // Tornooi
        public static List<Tournament> GetAll()
        {
            using (var db = new DbDartsmanagerContext())
            {
                var tornooien = db.Tournaments.Include(t => t.Adres).Include(t => t.Status).ToList();
                return tornooien;
            }
        }
        public static List<Tournament> GetTournamentsFromNameFilter(string filter)
        {
            using (var db = new DbDartsmanagerContext())
            {
                return db.Tournaments.Where(t => t.Naam.Contains(filter)).ToList();
            }
        }
        public static bool CheckExistingJaargang(string naam, int jaargang)
        {
            using (var db = new DbDartsmanagerContext())
            {
                bool bestaand_tornooi = false;
                var tornooi = db.Tournaments.FirstOrDefault(t => t.Naam == naam && t.Jaargang == jaargang);
                if (tornooi != null)
                {
                    bestaand_tornooi = true;
                }
                return bestaand_tornooi;
            }
        }
        public static bool CheckExistingJaargang(string naam, int jaargang, int id)
        {
            using (var db = new DbDartsmanagerContext())
            {
                bool bestaand_tornooi = false;
                var tornooi = db.Tournaments.FirstOrDefault(t => t.Naam == naam && t.Jaargang == jaargang && t.Id != id);
                if (tornooi != null)
                {
                    bestaand_tornooi = true;
                }
                return bestaand_tornooi;
            }
        }
        public static bool CheckMaxInschrijvingen(int max_inschrijvingen)
        {
            bool max_inschrijvingen_OK = false;
            // Inschrijvingen moet groter dan 3 zijn
            if (max_inschrijvingen > 3)
            {
                // Start met kleinste macht van 2
                int macht = 1;

                // Blijf verdubbelen zolang (macht * 2) nog een deler is van het getal (zoek de grootste macht van 2 die het getal exact deelt)
                while (max_inschrijvingen % (macht * 2) == 0)
                {
                    macht *= 2;
                }
                // Als het getal gelijk is aan die grootste deler, dan betekent dit dat het getal zelf een macht van 2 is (bv. 4, 8, 16, 32, ...)
                if (max_inschrijvingen == macht)
                {
                    max_inschrijvingen_OK = true;
                }
            }
            return max_inschrijvingen_OK;
        }
        public static void Update(Tournament tornooi)
        {
            try
            {
                using (var db = new DbDartsmanagerContext())
                {
                    var bestaandTornooi = db.Tournaments.FirstOrDefault(t => t.Id == tornooi.Id);
                    if (bestaandTornooi != null)
                    {
                        bestaandTornooi.Naam = tornooi.Naam;
                        bestaandTornooi.AdresId = tornooi.AdresId;
                        bestaandTornooi.Jaargang = tornooi.Jaargang;
                        bestaandTornooi.Datum = tornooi.Datum;
                        bestaandTornooi.StatusId = tornooi.StatusId;
                        bestaandTornooi.MaxInschrijvingen = tornooi.MaxInschrijvingen;
                        bestaandTornooi.AantalRondes = tornooi.AantalRondes;
                        bestaandTornooi.ActieveRonde = tornooi.ActieveRonde;
                        bestaandTornooi.ExtraInfo = tornooi.ExtraInfo;
                        db.SaveChanges();
                    }
                }
            }
            catch
            {
                throw new InvalidOperationException("De wijzigingen konden niet worden opgeslagen. Controleer of het tornooi nog wordt gebruikt in andere gegevens.");
            }
        }
        public static void Add(Tournament tornooi)
        {
            try
            {
                using (var db = new DbDartsmanagerContext())
                {
                    // start status zetten op niet gestart
                    var status = db.Statuses.First();
                    if (status != null)
                    {
                        tornooi.StatusId = status.Id;
                    }
                    db.Tournaments.Add(tornooi);
                    db.SaveChanges();
                }
            }
            catch
            {
                throw new InvalidOperationException("Kon het tornooi niet toevoegen. Controleer of de naam uniek is of de databaseverbinding juist is.");
            }
        }
        public static void Remove(Tournament tornooi)
        {
            try
            {
                using (var db = new DbDartsmanagerContext())
                {
                    var bestaandTornooi = db.Tournaments.FirstOrDefault(t => t.Id == tornooi.Id);
                    if (bestaandTornooi != null)
                    {
                        db.Tournaments.Remove(bestaandTornooi);
                        db.SaveChanges();
                    }
                }
            }
            catch
            {
                throw new InvalidOperationException("Dit tornooi kan niet worden verwijderd omdat hij nog gekoppeld is aan andere gegevens.");
            }
        }

        public static void CreateGameSchedule(Tournament tornooi)
        {
            try
            {
                using (var db = new DbDartsmanagerContext())
                {
                    var bestaandTornooi = db.Tournaments.FirstOrDefault(t => t.Id == tornooi.Id);
                    if (bestaandTornooi != null)
                    {
                       // Aantal inschrijvingen opzoeken


                        db.SaveChanges();
                    }
                }
            }
            catch
            {
                throw new InvalidOperationException("Kon het werdstrijdschema niet aanmaken.");
            }
        }

        // Registreren
        public static List<Registration> GetAllRegistrations(Tournament tornooi)
        {
            using (var db = new DbDartsmanagerContext())
            {
                var inschrijvingen = db.Registrations.Where(r => r.TournamentId == tornooi.Id).ToList();
                return inschrijvingen;
            }
        }
        public static void RegisterPlayer(Tournament tornooi, Player speler)
        {
            try
            {
                using (var db = new DbDartsmanagerContext())
                {
                    var bestaandTornooi = db.Tournaments.FirstOrDefault(t => t.Id == tornooi.Id);
                    var bestaandeSpeler = db.Players.FirstOrDefault(p => p.Id == speler.Id);
                    if (bestaandTornooi != null && bestaandeSpeler != null)
                    {
                        // Kijken of deze speler nog niet geregistreerd is
                        var bestaandRegistratie = db.Registrations.FirstOrDefault(r => r.PlayerId == bestaandeSpeler.Id && r.TournamentId == bestaandTornooi.Id);
                        if (bestaandRegistratie != null)
                        {
                            MessageBox.Show("Deze speler is al geregisteerd voor dit tornooi");
                            return;
                        }
                        // Kijken of de max inschrijvingen nog niet overschreden zijn
                        var inschrijvingen = GetAllRegistrations(tornooi);
                        if (inschrijvingen.Count >= tornooi.MaxInschrijvingen)
                        {
                            MessageBox.Show("Het maximum inschrijvingen voor dit tornooi is reeds bereikt");
                            return;
                        }
                        // Speler inschrijven voor tornooi
                        db.Registrations.Add(new Registration
                        {
                            PlayerId = bestaandeSpeler.Id,
                            TournamentId = bestaandTornooi.Id
                        });
                        MessageBox.Show($"De speler {speler.VoornaamNaam} werd ingeschreven voor het tornooi {tornooi.Naam} {tornooi.Jaargang}");
                        db.SaveChanges();
                    }
                }
            }
            catch
            {
                throw new InvalidOperationException("Deze speler kon niet geregistreerd worden.");
            }
        }
        public static void EndPlayerRegistration(Tournament tornooi, Player speler)
        {
            try
            {
                using (var db = new DbDartsmanagerContext())
                {
                    var bestaandRegistratie = db.Registrations.FirstOrDefault(r => r.PlayerId == speler.Id && r.TournamentId == tornooi.Id);
                    if (bestaandRegistratie != null)
                    {
                        MessageBoxResult result = MessageBox.Show($"Bent u zeker dat u deze inschrijving wenst te verwijderen?",
                                                  "Bevestig verwijdering",
                                                  MessageBoxButton.YesNo,
                                                  MessageBoxImage.Question);
                        if (result == MessageBoxResult.Yes)
                        {
                            RemoveRegistration(bestaandRegistratie);
                            MessageBox.Show($"De speler {speler.VoornaamNaam} werd uitgeschreven voor het tornooi {tornooi.Naam} {tornooi.Jaargang}");
                        }
                    }
                    else
                    {
                        MessageBox.Show($"De speler {speler.VoornaamNaam} was nog niet ingeschreven voor het tornooi {tornooi.Naam} {tornooi.Jaargang}");
                    }
                }
            }
            catch
            {
                throw new InvalidOperationException("Deze inschrijving kan niet verwijderd worden.");
            }
        }
        public static void RemoveRegistration(Registration inschrijving)
        {
            try
            {
                using (var db = new DbDartsmanagerContext())
                {
                    var bestaandRegistratie = db.Registrations.FirstOrDefault(r => r.Id == inschrijving.Id);
                    if (bestaandRegistratie != null)
                    {
                        db.Registrations.Remove(bestaandRegistratie);                        
                        db.SaveChanges();
                    }
                }
            }
            catch
            {
                throw new InvalidOperationException("Deze inschrijving kan niet worden verwijderd omdat hij nog gekoppeld is aan andere gegevens.");
            }
        }

        // Status
        public static List<Status> GetAllStatus()
        {
            using (var db = new DbDartsmanagerContext())
            {
                var statuses = db.Statuses.ToList();
                return statuses;
            }
        }
        public static void UpdateStatus(Status status)
        {
            try
            {
                using (var db = new DbDartsmanagerContext())
                {
                    var bestaandeStatus = db.Statuses.FirstOrDefault(s => s.Id == status.Id);
                    if (bestaandeStatus != null)
                    {
                        bestaandeStatus.Naam = status.Naam;
                        db.SaveChanges();
                    }
                }
            }
            catch
            {
                throw new InvalidOperationException("De wijzigingen konden niet worden opgeslagen. Controleer of de status nog wordt gebruikt in andere gegevens.");
            }
        }
        public static void AddStatus(Status status)
        {
            try
            {
                using (var db = new DbDartsmanagerContext())
                {
                    db.Statuses.Add(status);
                    db.SaveChanges();
                }
            }
            catch
            {
                throw new InvalidOperationException("Kon de status niet toevoegen. Controleer of de naam uniek is of de databaseverbinding juist is.");
            }
        }
        public static void RemoveStatus(Status status)
        {
            try
            {
                using (var db = new DbDartsmanagerContext())
                {
                    var bestaandeStatus = db.Statuses.FirstOrDefault(s => s.Id == status.Id);
                    if (bestaandeStatus != null)
                    {
                        db.Statuses.Remove(bestaandeStatus);
                        db.SaveChanges();
                    }
                }
            }
            catch
            {
                throw new InvalidOperationException("Deze status kan niet worden verwijderd omdat hij nog gekoppeld is aan andere gegevens.");
            }
        }

        
    }
}
