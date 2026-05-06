using Dartsmanager.Data;
using Dartsmanager.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

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
        public static List<Tournament> GetAllOpen()
        {
            using (var db = new DbDartsmanagerContext())
            {
                var tornooien = db.Tournaments.Include(t => t.Adres).Include(t => t.Status).Where(t => t.Status != null && t.Status.Naam.Contains("Niet gestart")).ToList();
                return tornooien;
            }
        }
        public static List<Tournament> GetAll(Player speler)
        {
            using (var db = new DbDartsmanagerContext())
            {
                var tornooien = db.Tournaments
                    .Include(t => t.Adres)
                    .Include(t => t.Status)
                    .Include(t => t.Registrations)
                    .ThenInclude(r => r.Player)
                    .Where(t => t.Registrations.Any(r => r.PlayerId == speler.Id))
                    .ToList();
                return tornooien;
            }
        }
        public static int? GetRonde(Tournament tornooi)
        {
            using (var db = new DbDartsmanagerContext())
            {
                var gezocht_tornooi = db.Tournaments.FirstOrDefault(t => t.Id == tornooi.Id);
                if (gezocht_tornooi != null)
                {
                    return gezocht_tornooi.ActieveRonde;
                }
                return 0;
            }
        }
        public static List<Tournament> GetTournamentsFromNameFilter(string filter, Player? speler = null)
        {
            using (var db = new DbDartsmanagerContext())
            {
                if (speler != null)
                {
                    return db.Tournaments
                    .Include(t => t.Adres)
                    .Include(t => t.Status)
                    .Include(t => t.Registrations)
                    .ThenInclude(r => r.Player)
                    .Where(t => t.Naam.Contains(filter) && t.Registrations.Any(r => r.PlayerId == speler.Id))
                    .ToList();
                }
                return db.Tournaments
                    .Include(t => t.Adres)
                    .Include(t => t.Status)
                    .Where(t => t.Naam.Contains(filter)).ToList();
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
                    var bestaandTornooi = db.Tournaments.Include(t => t.Status).FirstOrDefault(t => t.Id == tornooi.Id);
                    if (bestaandTornooi != null)
                    {
                        // Enkel tornooien verwijderen die nog niet gestart of afgelopen zijn!
                        if (bestaandTornooi.Status == null || (bestaandTornooi.Status != null && bestaandTornooi.Status.Naam == "Niet gestart"))
                        {
                            db.Tournaments.Remove(bestaandTornooi);
                            db.SaveChanges();
                        }
                        else
                        {
                            MessageBox.Show("Dit tornooi kan niet langer verwijderd worden");
                        }
                    }
                }
            }
            catch
            {
                throw new InvalidOperationException("Dit tornooi kan niet worden verwijderd omdat hij nog gekoppeld is aan andere gegevens.");
            }
        }
        public static Status? Start(Tournament tornooi)
        {
            try
            {
                using (var db = new DbDartsmanagerContext())
                {
                    // Enkel starten indien nog niet gestart werd
                    if ((tornooi.StatusId != null && tornooi.Status != null && tornooi.Status.Naam == "Niet gestart") || tornooi.Status == null)
                    {
                        // kijken of max inschrijvingen werd bereikt en vragen of er met dummies gespeeld wilt worden
                        if (tornooi.MaxInschrijvingen > GetAllRegistrations(tornooi).Count)
                        {
                            MessageBoxResult result = MessageBox.Show($"U bent nog niet aan het max aantal inschrijvingen, wilt u alsnog starten met dummy spelers?",
                                                          "Bevestig start",
                                                          MessageBoxButton.YesNo,
                                                          MessageBoxImage.Question);
                            if (result == MessageBoxResult.No)
                            {
                                return tornooi.Status;
                            }
                        }
                        // De status met naam "Gestart" ophalen en wijzigen in tornooi
                        var status = db.Statuses.FirstOrDefault(s => s.Naam == "Gestart");
                        var bestaandTornooi = db.Tournaments.FirstOrDefault(t => t.Id == tornooi.Id);
                        if (status != null && bestaandTornooi != null )
                        {
                            bestaandTornooi.StatusId = status.Id;
                            bestaandTornooi.ActieveRonde = 1;
                            // Aantal rondes bepalen
                            int rondes = 1;
                            if (bestaandTornooi.MaxInschrijvingen != null)
                            { 
                                rondes = (int)Math.Log2((int)bestaandTornooi.MaxInschrijvingen);
                            }
                            bestaandTornooi.AantalRondes = rondes;
                            db.SaveChanges();
                            MessageBox.Show("Het tornooi is gestart.");
                            return status;
                        }

                    }
                    else
                    {
                        MessageBox.Show("Dit tornooi kan niet langer gestart worden");
                    }
                    return tornooi.Status;
                }
            }
            catch
            {
                throw new InvalidOperationException("Kon het tornooi niet toevoegen. Controleer of de naam uniek is of de databaseverbinding juist is.");
            }
        }
        public static Status? Open(Tournament tornooi)
        {
            try
            {
                using (var db = new DbDartsmanagerContext())
                {
                    // Enkel openen indien gestart werd en nog geen wedstrijdschema gemaakt is
                    if (tornooi.Status != null && tornooi.Status.Naam == "Gestart")
                    {
                        var groepen = db.Groups.Where(g => g.TournamentId  == tornooi.Id).ToList();
                        if (groepen != null && groepen.Count > 0)
                        {
                            MessageBox.Show("Er zijn al groepen gecreërd, u kan het tornooi niet meer open zetten.");
                            return tornooi.Status;
                        }
                        // De status met naam "Niet gestart" ophalen en wijzigen in tornooi
                        var status = db.Statuses.FirstOrDefault(s => s.Naam == "Niet gestart");
                        var bestaandTornooi = db.Tournaments.FirstOrDefault(t => t.Id == tornooi.Id);
                        if (status != null && bestaandTornooi != null)
                        {
                            bestaandTornooi.StatusId = status.Id;
                            bestaandTornooi.ActieveRonde = 0;
                            db.SaveChanges();
                            MessageBox.Show("Het tornooi is terug open gezet.");
                            return status;
                        }

                    }
                    else
                    {
                        MessageBox.Show("Dit tornooi kan niet langer open gezet worden");
                    }
                    return tornooi.Status;
                }
            }
            catch
            {
                throw new InvalidOperationException("Kon het tornooi niet open zetten. Controleer of de naam uniek is of de databaseverbinding juist is.");
            }
        }

        public static List<Group> GetAllGroups(Tournament tornooi)
        {
            using (var db = new DbDartsmanagerContext())
            {
                var groepen = db.Groups.Where(g => g.TournamentId == tornooi.Id).ToList();
                return groepen;
            }
        }
        public static void CreateGroups(Tournament tornooi)
        {
            try
            {
                using (var db = new DbDartsmanagerContext())
                {
                    var bestaandTornooi = db.Tournaments.FirstOrDefault(t => t.Id == tornooi.Id);
                    if (bestaandTornooi != null)
                    {
                        // Kijken of er al groepen zijn gemaakt, anders niets bijmaken
                        var groepen = GetAllGroups(tornooi);
                        if (groepen.Count > 0)
                        {
                            MessageBox.Show("Er zijn al groepen aangemaakt in dit tornooi!");
                            return;
                        }


                        // Aantal speler inschrijvingen opzoeken
                        var inschrijvingen = GetAllPlayerRegistrations(tornooi);
                        if (inschrijvingen != null && tornooi.MaxInschrijvingen != null)
                        {
                            if (inschrijvingen.Count == 0)
                            {
                                MessageBox.Show("Er zijn geen inschrijvingen geregistreerd!");
                                return;
                            }
                            // Groepen aanmaken
                            int aantal_groepen = 0;
                            if ((int)tornooi.MaxInschrijvingen % 4 == 0)
                            {
                                aantal_groepen = (int)tornooi.MaxInschrijvingen / 4;
                                for (int i = 0; i < aantal_groepen; i++)
                                {
                                    CreateGroup(tornooi, i + 1);
                                }
                            }
                            else
                            {
                                MessageBox.Show("Het max aantal inschrijvingen moet deelbaar zijn door 4!");
                                return;
                            }

                            // Kijken of er dummies nodig zijn
                            var inschrijvingen_dummy = GetAllDummyRegistrations(tornooi);
                            int aantal_dummy = (int)tornooi.MaxInschrijvingen - inschrijvingen.Count - inschrijvingen_dummy.Count;
                            if (aantal_dummy > 0)
                            {
                                // Dummies maken
                                for (int i = 0; i < aantal_dummy; i++)
                                {
                                    PlayerService.AddDummy(i + 1);
                                }
                                // Dummies inschrijven voor het tornooi
                                for (int i = 0; i < aantal_dummy; i++)
                                {
                                    var dummy = db.Players.FirstOrDefault(p => p.Voornaam == "Dummy" && p.Naam == $"{i + 1}");
                                    if (dummy != null)
                                    {
                                        RegisterPlayer(tornooi, dummy);
                                    }
                                }
                            }

                            // Eerst spelers evenredig verdelen in groepen
                            // Random volgorde maken
                            var random = new Random();
                            for (int i = inschrijvingen.Count - 1; i > 0; i--)
                            {
                                int j = random.Next(i + 1);
                                var temp = inschrijvingen[i];
                                inschrijvingen[i] = inschrijvingen[j];
                                inschrijvingen[j] = temp;
                            }
                            // Groepen en spelers ophalen
                            groepen = GetAllGroups(tornooi);
                            var spelers = db.Players.ToList();
                            // Al de spelers inschrijvingen doorlopen
                            for (int i = 0; i < inschrijvingen.Count; i++)
                            {
                                // De rest bij de deling door het aantal groepen (=totaal aantal/4) bepaalt de groepsnummer (rest + 1)
                                int groepIndex = i % aantal_groepen + 1;

                                var groep = groepen.FirstOrDefault(g => g.GroepNummer == groepIndex);
                                if (groep != null)
                                {
                                    var speler = spelers.FirstOrDefault(p => p.Id == inschrijvingen[i].PlayerId);
                                    if (speler != null)
                                    {
                                        AddPlayerToGroup(groep, speler);
                                    }
                                    
                                }
                            }

                            // Dan dummies verdelen in groepen
                            inschrijvingen_dummy.Clear();
                            inschrijvingen_dummy = GetAllDummyRegistrations(tornooi);
                            int dummyIndex = 0;
                            for (int i = 0; i < groepen.Count; i++)
                            {
                                var groep = groepen[i];

                                // Tel hoeveel spelers er al in zitten en eventuele vrije plaatsen tellen
                                var huidigeSpelers = db.GroupPlayers.Where(gp => gp.GroupId == groep.Id).Count();
                                int vrijePlaatsen = 4 - huidigeSpelers;

                                for (int j = 0; j < vrijePlaatsen && dummyIndex < inschrijvingen_dummy.Count; j++)
                                {
                                    var dummy = db.Players.FirstOrDefault(p => p.Id == inschrijvingen_dummy[dummyIndex].PlayerId);
                                    if (dummy != null)
                                    {
                                        AddPlayerToGroup(groep, dummy);
                                    }
                                    dummyIndex++;
                                }
                            }

                            db.SaveChanges();
                            MessageBox.Show("Groepen aangemaakt");
                        }

                       
                    }
                }
            }
            catch
            {
                throw new InvalidOperationException("Kon de groepen niet aanmaken.");
            }
        }
        public static void CreateWedstrijdSchema(Tournament tornooi)
        {
            try
            {
                using (var db = new DbDartsmanagerContext())
                {
                    var groupen = db.Groups.Where(g => g.TournamentId == tornooi.Id).ToList();

                    // Kijken of er al wedstrijden zijn gemaakt, anders niets bijmaken
                    var wedstrijden = GameService.GetAll(tornooi);
                    if (wedstrijden.Count > 0)
                    {
                        MessageBox.Show("Er zijn al wedstrijden aangemaakt in deze groepen!");
                        return;
                    }

                    foreach (var groep in groupen)
                    {
                        // Spelerid's selecteren van al de spelers in de groep
                        var speler_Ids = db.GroupPlayers.Where(gp => gp.GroupId == groep.Id).Select(gp => gp.PlayerId).ToList();
                        // elke speler tegen elke andere speler
                        for (int i = 0; i < speler_Ids.Count; i++)
                        {
                            for (int j = i + 1; j < speler_Ids.Count; j++)
                            {
                                var wedstrijd = new Game
                                {
                                    TournamentId = tornooi.Id,
                                    GroupId = groep.Id,
                                    Player1Id = speler_Ids[i],
                                    Player2Id = speler_Ids[j],
                                    Ronde = 1
                                };
                                GameService.Add(wedstrijd);
                            }
                        }
                    }
                    tornooi.ActieveRonde = 1;
                    // Aantal rondes bepalen
                    int rondes = 1;
                    if (tornooi.MaxInschrijvingen != null)
                    {
                        rondes = (int)Math.Log2((int)tornooi.MaxInschrijvingen);
                    }
                    tornooi.AantalRondes = rondes;
                    Update(tornooi);
                    db.SaveChanges();
                    MessageBox.Show("Wedstrijdschema aangemaakt");
                }
            }
            catch
            {
                throw new InvalidOperationException("Kon het schema niet aanmaken.");
            }

            
        }
        public static int? GetActieveRonde(Tournament tornooi)
        {
            using (var db = new DbDartsmanagerContext())
            {
                if (tornooi != null)
                {
                    var gezocht_tornooi = db.Tournaments.FirstOrDefault(t => t.Id == tornooi.Id);
                    if (gezocht_tornooi != null && gezocht_tornooi.ActieveRonde != null)
                    {
                        return (int)gezocht_tornooi.ActieveRonde ;
                    }
                }
                return null;
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
        public static int? GetStatusId(Tournament? tornooi)
        {
            using (var db = new DbDartsmanagerContext())
            {
                if (tornooi != null)
                {
                    var gezocht_tornooi = db.Tournaments.FirstOrDefault(t => t.Id == tornooi.Id);
                    if (gezocht_tornooi != null)
                    {
                        return gezocht_tornooi.StatusId;
                    }
                }
                return null;
            }
        }
        public static Status? GetStatusById(int statusId)
        {
            using (var db = new DbDartsmanagerContext())
            {
                var gezochte_status = db.Statuses.FirstOrDefault(s => s.Id == statusId);
                if (gezochte_status != null)
                {
                    return gezochte_status;
                }
                return null;
            }
        }
        public static Status? GetStatusByName(string naam)
        {
            using (var db = new DbDartsmanagerContext())
            {
                var status = db.Statuses.FirstOrDefault(s => s.Naam == naam);
                return status;
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

        // Registreren
        public static List<Registration> GetAllRegistrations(Tournament tornooi)
        {
            using (var db = new DbDartsmanagerContext())
            {
                var inschrijvingen = db.Registrations.Where(r => r.TournamentId == tornooi.Id).Include(r => r.Player).ToList();
                return inschrijvingen;
            }
        }
        public static List<Registration> GetAllPlayerRegistrations(Tournament tornooi)
        {
            using (var db = new DbDartsmanagerContext())
            {
                var inschrijvingen = db.Registrations.Where(r => r.TournamentId == tornooi.Id && r.Player.IsDummy != 1).Include(r => r.Player).ToList();
                return inschrijvingen;
            }
        }
        public static List<Registration> GetAllDummyRegistrations(Tournament tornooi)
        {
            using (var db = new DbDartsmanagerContext())
            {
                var inschrijvingen = db.Registrations.Where(r => r.TournamentId == tornooi.Id && r.Player.IsDummy == 1).Include(r => r.Player).ToList();
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
                        if (bestaandRegistratie != null )
                        {
                            if (bestaandRegistratie.Player.IsDummy != 1)
                            {
                                MessageBox.Show("Deze speler is al geregisteerd voor dit tornooi");
                            }
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
                    var bestaandRegistratie = db.Registrations.Include(r => r.Tournament).FirstOrDefault(r => r.PlayerId == speler.Id && r.TournamentId == tornooi.Id);
                    if (bestaandRegistratie != null)
                    {
                        // kijken of er al wedstrijden bestaan voor het tornooi
                        var wedstrijden = GameService.GetAll(bestaandRegistratie.Tournament);
                        if (wedstrijden.Count > 0)
                        {
                            MessageBox.Show("Er zijn al wedstrijden aangemaakt. U kan de speler niet meer verwijderen uit het tornooi!");
                            return;
                        }
                        // Bevestiging vragen
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

        
        // Groepen
        public static void CreateGroup(Tournament tornooi, int groepnummer)
        {
            try
            {
                using (var db = new DbDartsmanagerContext())
                {
                    Group groep = new Group
                    {
                        TournamentId = tornooi.Id,
                        GroepNummer = groepnummer
                    };
                    db.Groups.Add(groep);
                    db.SaveChanges();
                }
            }
            catch
            {
                throw new InvalidOperationException("Kon de groep niet toevoegen. Controleer of de naam uniek is of de databaseverbinding juist is.");
            }
        }
        public static void AddPlayerToGroup(Group groep, Player speler)
        {
            try
            {
                using (var db = new DbDartsmanagerContext())
                {
                    GroupPlayer groepspeler = new GroupPlayer
                    {
                        GroupId = groep.Id,
                        PlayerId = speler.Id
                    };
                    db.GroupPlayers.Add(groepspeler);
                    db.SaveChanges();
                }
            }
            catch
            {
                throw new InvalidOperationException("Kon de speler niet toevoegen aan de groep. Controleer of de naam uniek is of de databaseverbinding juist is.");
            }
        }
        public static List<Player> GetAllPlayersFromGroup(Group groep)
        {
            using (var db = new DbDartsmanagerContext())
            {
                var spelers = db.GroupPlayers.Where(gp => gp.GroupId == groep.Id).Include(gp => gp.Player).Select(gp => gp.Player).ToList();
                return spelers;
            }
        }
        public static Group? GetGroupFromGame(Game wedstrijd)
        {
            using (var db = new DbDartsmanagerContext())
            {
                var groep = db.Groups.FirstOrDefault(g => g.Games.Contains(wedstrijd));
                return groep;
            }
        }
        public static Group? GetGroup(Tournament tornooi, Player speler)
        {
            using (var db = new DbDartsmanagerContext())
            {
                var groep = db.Groups.FirstOrDefault(g => g.GroupPlayers.Any(gp => gp.Player == speler) && g.Tournament.Id == tornooi.Id);
                return groep;
            }
        }
        public static int GetSetsPerGroupsPlayer(Group groep, Player speler)
        {
            using (var db = new DbDartsmanagerContext())
            {
                var sets = db.GameScores.Where(gs => gs.PlayerId == speler.Id && gs.Game.GroupId == groep.Id).Sum(gs => gs.SetsWon.GetValueOrDefault());
                return sets;
            }
        }
        public static int GetLegsPerGroupsPlayer(Group groep, Player speler)
        {
            using (var db = new DbDartsmanagerContext())
            {
                var legs = db.GameScores.Where(gs => gs.PlayerId == speler.Id && gs.Game.GroupId == groep.Id).Sum(gs => gs.LegsWon.GetValueOrDefault());
                return legs;
            }
        }
        public static int Get180PerGroupsPlayer(Group groep, Player speler)
        {
            using (var db = new DbDartsmanagerContext())
            {
                var aantal_180 = db.GameScores.Where(gs => gs.PlayerId == speler.Id && gs.Game.GroupId == groep.Id).Sum(gs => gs.Aantal180.GetValueOrDefault());
                return aantal_180;
            }
        }
        public static double GetGemiddeldePerGroupsPlayer(Group groep, Player speler)
        {
            using (var db = new DbDartsmanagerContext())
            {
                var gemiddelde = db.GameScores.Where(gs => gs.PlayerId == speler.Id && gs.Game.GroupId == groep.Id).Average(gs => gs.Gemiddelde.GetValueOrDefault());
                gemiddelde = Math.Round(gemiddelde,1);
                return gemiddelde;
            }
        }
        public static void EndGroupFase(Tournament tornooi)
        {
            try
            {
                using (var db = new DbDartsmanagerContext())
                {
                    if (tornooi.ActieveRonde == 1)
                    {
                        // Kijken of elke wedstrijd een winnaar heeft
                        var wedstrijden = GameService.GetAll(tornooi, 1);
                        foreach(var wedstrijd in wedstrijden)
                        {
                            if (GameService.GetGameResult(wedstrijd) == null)
                            {
                                MessageBox.Show("Nog niet al de wedstrijden zijn afgerond. Zorg dat elke wedstrijd een winnaar heeft.");
                                return;
                            }                       
                        }
                        // 4 groepen maken uit de groepsfase: 2 voor de winnaars en 2 voor de verliezers
                        List<Player> plaats1 = new List<Player>();
                        List<Player> plaats2 = new List<Player>();
                        List<Player> plaats3 = new List<Player>();
                        List<Player> plaats4 = new List<Player>();
                        var groepen = db.Groups.Where(g => g.TournamentId == tornooi.Id).ToList();
                        foreach (var groep in groepen)
                        {
                            // De groep sorteren op score
                            List<GroupPlayerInfo> spelersinfo = GetPlayerRanking(groep);
                            for (int i = 0; i < spelersinfo.Count; i++)
                            {
                                // De eerste 2 spelers uit elke gesorteerde groep zijn de winnaars
                                if (i < 1)
                                {
                                    plaats1.Add(spelersinfo[i].Speler);
                                }
                                else if (i < 2)
                                {
                                    plaats2.Add(spelersinfo[i].Speler);
                                }
                                else if (i < 3)
                                {
                                    plaats3.Add(spelersinfo[i].Speler);
                                }
                                else if (i < 4)
                                {
                                    plaats4.Add(spelersinfo[i].Speler);
                                }
                            }
                        }
                        
                        // Plaats 1 & 2 omgedraaid matchen, zodat dezelfde groepspelers pas in de finale elkaar ontmoeten
                        for (int i = 0; i < plaats1.Count; i++)
                        {
                            var wedstrijd = new Game
                            {
                                TournamentId = tornooi.Id,
                                Player1Id = plaats1[i].Id,
                                Player2Id = plaats2[plaats2.Count - i - 1].Id,
                                Ronde = 2
                            };
                            GameService.Add(wedstrijd);
                        }
                        // Plaats 3 & 4 omgedraaid matchen, zodat dezelfde groepspelers pas in de finale elkaar ontmoeten
                        for (int i = 0; i < plaats3.Count; i++)
                        {
                            var wedstrijd = new Game
                            {
                                TournamentId = tornooi.Id,
                                Player1Id = plaats3[i].Id,
                                Player2Id = plaats4[plaats4.Count - i - 1].Id,
                                Ronde = 2
                            };
                            GameService.Add(wedstrijd);
                        }
                        db.SaveChanges();
                        // Ronde met 1 verhogen
                        tornooi.ActieveRonde++;
                        Update(tornooi);
                    }
                }
            }
            catch
            {
                throw new InvalidOperationException("Kon de groepsfase niet eindigen. Controleer of de naam uniek is of de databaseverbinding juist is.");
            }
        }
        public static void NextRound(Tournament tornooi)
        {
            try
            {
                using (var db = new DbDartsmanagerContext())
                {
                    if (tornooi.ActieveRonde!= null && tornooi.ActieveRonde > 1)
                    {
                        // Kijken of elke wedstrijd een winnaar heeft
                        var wedstrijden = GameService.GetAll(tornooi, (int)tornooi.ActieveRonde);
                        List<Player> winnaars = new List<Player>();
                        List<Player> verliezers = new List<Player>();
                        foreach (var wedstrijd in wedstrijden)
                        {
                            var resultaat = GameService.GetGameResult(wedstrijd);
                            if (resultaat != null)
                            {
                                winnaars.Add(resultaat.Value.winnaar);
                                verliezers.Add(resultaat.Value.verliezer);
                            }
                            else // Methode afbreken bij een gelijke stand
                            {
                                MessageBox.Show("Nog niet al de wedstrijden zijn afgerond. Zorg dat elke wedstrijd een winnaar heeft.");
                                return;
                            }
                        }
                        // Kijken of dit de laatse ronde was
                        if (tornooi.ActieveRonde == tornooi.AantalRondes)
                        {
                            MessageBox.Show("Het tornooi werd afgesloten.");
                            var status = GetStatusByName("Afgelopen");
                            if (status != null)
                            {
                                tornooi.StatusId = status.Id;
                                Update(tornooi);
                            }
                            // Ranking herberekenen
                            PlayerService.CalculateCompleteRanking();
                            return;
                        }
                        // Wedstrijden maken voor de winnaars (indien er meer dan 1 winaar is
                        if (winnaars.Count > 1)
                        {
                            for (int i = 0; i < winnaars.Count; i += 2)
                            {
                                

                                var wedstrijd = new Game
                                {
                                    TournamentId = tornooi.Id,
                                    Player1Id = winnaars[i].Id,
                                    Player2Id = winnaars[i + 1].Id,
                                    Ronde = tornooi.ActieveRonde + 1
                                };
                                GameService.Add(wedstrijd);
                            }
                        }

                        db.SaveChanges();
                        // Ronde met 1 verhogen
                        tornooi.ActieveRonde++;
                        Update(tornooi);
                    }
                }
            }
            catch
            {
                throw new InvalidOperationException("Kon de volgende ronde niet aanmaken. Controleer of de naam uniek is of de databaseverbinding juist is.");
            }
        }

        // Statistieken
        public static List<GroupPlayerInfo> GetPlayerRanking(Group groep)
        {
            var spelers = GetAllPlayersFromGroup(groep);
            List<GroupPlayerInfo> spelersinfo = new List<GroupPlayerInfo>();
            foreach (Player sp in spelers)
            {
                spelersinfo.Add(new GroupPlayerInfo
                {
                    Speler = sp,
                    GroepSets = GetSetsPerGroupsPlayer(groep, sp),
                    GroepLegs = GetLegsPerGroupsPlayer(groep, sp),
                    Groep180 = Get180PerGroupsPlayer(groep, sp),
                    GroepGemiddelde = GetGemiddeldePerGroupsPlayer(groep, sp),
                });
            }
            // Sorteren
            spelersinfo = spelersinfo
                .OrderByDescending(s => s.GroepSets)
                .ThenByDescending(s => s.GroepLegs)
                .ThenByDescending(s => s.Groep180)
                .ThenByDescending(s => s.GroepGemiddelde)
                .ToList();

            return spelersinfo;
        }
        public static (int groepsfase, int wedstrijden_gewonnen, int legs, int aantal_180, double gemiddelde) GetTournamentstatistics(Tournament tornooi, Player speler)
        {
            var wedstrijden = GameService.GetAll(tornooi, speler);
            int groepsfase = 0;
            int totaal_gewonnen = 0;
            int legs = 0;
            int aantal_180 = 0;
            double gemiddelde = 0;
            double totaal_gemiddelde = 0;
            int aantal_gemiddelde = 0;
            // De groep zoeken waar de speler in zat en zijn resultaat binnen de groep ophalen
            var groep = GetGroup(tornooi, speler);
            if (groep == null)
            {
                MessageBox.Show("null");
                return (0,0,0,0,0);
            }
            // De groep sorteren op score
            List < GroupPlayerInfo > spelersinfo = GetPlayerRanking(groep);
            for (int i = 0; i < spelersinfo.Count; i++)
            {
                // De eerste 2 spelers uit elke gesorteerde groep zijn de winnaars
                if (i < 2)
                {
                    groepsfase = 1;
                }                
                else if (i < 4)
                {
                    groepsfase = 2;
                }
            }

            // Al de wedstrijden doorlopen
            foreach (var wedstrijd in wedstrijden)
            {
                // resultaat ophalen en kijken of speler gewonnen had
                var resultaat = GameService.GetGameResult(wedstrijd);
                if (resultaat != null && resultaat.Value.winnaar.Id == speler.Id)
                {
                    totaal_gewonnen ++;
                }
                legs += GameService.GetLegsFromGamescore(wedstrijd, speler);
                aantal_180 += GameService.Get180sFromGamescore(wedstrijd, speler);
                totaal_gemiddelde += GameService.GetGemiddeldeFromGamescore(wedstrijd, speler);
                aantal_gemiddelde++;
            }
            gemiddelde = totaal_gemiddelde / aantal_gemiddelde;
            return (groepsfase, totaal_gewonnen, legs, aantal_180, gemiddelde);
        }
    }
}
