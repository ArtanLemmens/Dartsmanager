using Dartsmanager.Data;
using Dartsmanager.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace Dartsmanager.Services
{
    public class GameService
    {
        public static List<Game> GetAll()
        {
            using (var db = new DbDartsmanagerContext())
            {
                var wedstrijden = db.Games.ToList();
                return wedstrijden;
            }
        }
        public static List<Game> GetAll(Tournament tornooi)
        {
            using (var db = new DbDartsmanagerContext())
            {
                var wedstrijden = db.Games.Where(g => g.TournamentId == tornooi.Id).ToList();
                return wedstrijden;
            }
        }
        public static List<Game> GetAll(Group groep)
        {
            using (var db = new DbDartsmanagerContext())
            {
                var wedstrijden = db.Games.Where(g => g.GroupId == groep.Id).ToList();
                return wedstrijden;
            }
        }
        public static List<Game> GetAll(Player speler)
        {
            using (var db = new DbDartsmanagerContext())
            {
                var wedstrijden = db.Games.Where(g => g.Player1Id == speler.Id || g.Player2Id == speler.Id).ToList();
                return wedstrijden;
            }
        }
        public static void Add(Game wedstrijd)
        {
            try
            {
                using (var db = new DbDartsmanagerContext())
                {
                    // Als deze wedstrijd al bestaat niet nogmaals creëeren
                    bool bestaande_wedstrijd = db.Games.Any(g =>
                    g.TournamentId == wedstrijd.TournamentId &&
                    g.GroupId == wedstrijd.GroupId &&
                    ((g.Player1Id == wedstrijd.Player1Id && g.Player2Id == wedstrijd.Player2Id) || (g.Player1Id == wedstrijd.Player2Id && g.Player2Id == wedstrijd.Player1Id)));
                    if (bestaande_wedstrijd == true)
                    {
                        return;
                    }
                    db.Games.Add(wedstrijd);
                    db.SaveChanges();
                    // Wedstrijdscores aanmaken
                    AddScore(wedstrijd, wedstrijd.Player1Id);
                    AddScore(wedstrijd, wedstrijd.Player2Id);
                }
            }
            catch
            {
                throw new InvalidOperationException("Kon de wedstrijd niet toevoegen. Controleer of de naam uniek is of de databaseverbinding juist is.");
            }
        }


        // GameScores
        public static void AddScore(Game wedstrijd, int speler_id)
        {
            try
            {
                using (var db = new DbDartsmanagerContext())
                {
                    var score = new GameScore
                    {
                        GameId = wedstrijd.Id,
                        PlayerId = speler_id,
                        SetsWon = 0,
                        LegsWon = 0,
                        Aantal180 = 0,
                        Gemiddelde = 0
                    };

                    db.GameScores.Add(score);
                    db.SaveChanges();
                }
            }
            catch
            {
                throw new InvalidOperationException("Kon de score niet toevoegen. Controleer of de naam uniek is of de databaseverbinding juist is.");
            }
        }

    }
}
