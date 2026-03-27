using Dartsmanager.Data;
using Dartsmanager.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dartsmanager.Services
{
    public class StatusService
    {
        public static List<Status> GetAll()
        {
            using (var db = new DbDartsmanagerContext())
            {
                var statuses = db.Statuses.ToList();
                return statuses;
            }
        }
        public static void Update(Status status)
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
        public static void Add(Status status)
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
        public static void Remove(Status status)
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
                throw new InvalidOperationException("Deze status kan niet worden verwijderd omdat hij nog gekoppeld is aan een project of andere gegevens.");
            }
        }
    }
}
