using Dartsmanager.Data;
using Dartsmanager.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace Dartsmanager.Services
{
    public class GameService
    {
        public static List<Game> GetAll()
        {
            using (var db = new DbDartsmanagerContext())
            {
                var wedstrijden = db.Games.Include(g => g.Tournament)
                    .Include(g => g.Player1)
                    .Include(g => g.Player2)
                    .ToList();
                return wedstrijden;
            }
        }
        public static List<Game> GetAll(Tournament? tornooi)
        {
            using (var db = new DbDartsmanagerContext())
            {
                if (tornooi == null)
                {
                    return db.Games.Include(g => g.Tournament)
                    .Include(g => g.Player1)
                    .Include(g => g.Player2)
                    .ToList();
                }
                var wedstrijden = db.Games.Include(g => g.Tournament)
                    .Include(g => g.Player1)
                    .Include(g => g.Player2)
                    .Where(g => g.TournamentId == tornooi.Id)
                    .ToList();
                return wedstrijden;
            }
        } 
        public static List<Game> GetAll(Tournament tornooi, int ronde)
        {
            using (var db = new DbDartsmanagerContext())
            {
                var wedstrijden = db.Games
                    .Include(g => g.Player1)
                    .Include(g => g.Player2)
                    .Include(g => g.Tournament)
                    .Where(g => g.TournamentId == tornooi.Id && g.Ronde == ronde)
                    .ToList();
                return wedstrijden;
            }
        }
        public static List<Game> GetAll(Group groep)
        {
            using (var db = new DbDartsmanagerContext())
            {
                var wedstrijden = db.Games.Where(g => g.GroupId == groep.Id).Include(g => g.Player1).Include(g => g.Player2).ToList();
                return wedstrijden;
            }
        }
        public static List<Game> GetAll(Player speler)
        {
            using (var db = new DbDartsmanagerContext())
            {
                var wedstrijden = db.Games
                    .Include(g => g.Tournament)
                    .Include(g => g.Player1)
                    .Include(g => g.Player2)
                    .Where(g => g.Player1Id == speler.Id || g.Player2Id == speler.Id).ToList();
                return wedstrijden;
            }
        }
        public static List<Game> GetAll(Tournament tornooi, Player speler)
        {
            using (var db = new DbDartsmanagerContext())
            {
                var wedstrijden = db.Games
                    .Include(g => g.Tournament)
                    .Include(g => g.Player1)
                    .Include(g => g.Player2)
                    .Where(g => g.TournamentId == tornooi.Id && (g.Player1Id == speler.Id || g.Player2Id == speler.Id)).ToList();
                return wedstrijden;
            }
        }
        public static List<Game> GetAllfromKOstage(Tournament tornooi)
        {
            using (var db = new DbDartsmanagerContext())
            {
                var wedstrijden = db.Games.Where(g => g.TournamentId == tornooi.Id && tornooi.ActieveRonde != null && tornooi.ActieveRonde > 1).ToList();
                return wedstrijden;
            }
        }
        public static List<Game> GetGamesFromNameFilter(string filter, Player? speler = null, Tournament? tornooi = null)
        {
            using (var db = new DbDartsmanagerContext())
            {
                return db.Games
                    .Include(g => g.Tournament)
                    .Include(g => g.Player1)
                    .Include(g => g.Player2)
                    .Where(g => ((g.Tournament != null && g.Tournament.Naam.Contains(filter))
                    || (g.Player1 != null && (g.Player1.Voornaam.Contains(filter) || g.Player1.Naam.Contains(filter)))
                    || (g.Player2 != null && (g.Player2.Voornaam.Contains(filter) || g.Player2.Naam.Contains(filter))))
                    && (speler == null || g.Player1Id == speler.Id || g.Player2Id == speler.Id)
                    && (tornooi == null || g.TournamentId == tornooi.Id))
                    .ToList();
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
        public static List<GameScore> GetAllScores(Group groep)
        {
            using (var db = new DbDartsmanagerContext())
            {
                var gameIds = db.Games.Where(g => g.GroupId == groep.Id).Select(g => g.Id);
                var wedstrijdscores = db.GameScores.Where(gs => gameIds.Contains(gs.GameId)).ToList();
                return wedstrijdscores;
            }
        }
        public static List<GameScore> GetAllScores(Tournament tornooi)
        {
            using (var db = new DbDartsmanagerContext())
            {
                var gameIds = db.Games.Where(g => g.TournamentId == tornooi.Id).Select(g => g.Id);
                var wedstrijdscores = db.GameScores.Where(gs => gameIds.Contains(gs.GameId)).ToList();
                return wedstrijdscores;
            }
        }
        public static List<GameScore> GetAllScores(Game wedstrijd)
        {
            using (var db = new DbDartsmanagerContext())
            {
                var wedstrijdscores = db.GameScores.Include(gs => gs.Player).Where(gs => gs.GameId == wedstrijd.Id).ToList();
                return wedstrijdscores;
            }
        }
        public static GameScore? GetScore(Game wedstrijd, Player speler)
        {
            using (var db = new DbDartsmanagerContext())
            {
                var wedstrijdscore = db.GameScores.FirstOrDefault(gs => gs.GameId == wedstrijd.Id && gs.PlayerId == speler.Id);
                if (wedstrijdscore != null)
                {
                    return wedstrijdscore;
                };
                return null;
            }
        }
        public static int GetLegsFromGamescore(Game wedstrijd, Player speler)
        {
            using (var db = new DbDartsmanagerContext())
            {
                int legs = db.GameScores.Where(gs => gs.GameId == wedstrijd.Id && gs.PlayerId == speler.Id).Select(gs => gs.LegsWon.GetValueOrDefault()).FirstOrDefault();
                return legs;
            }
        }
        public static int Get180sFromGamescore(Game wedstrijd, Player speler)
        {
            using (var db = new DbDartsmanagerContext())
            {
                int aantal180 = db.GameScores.Where(gs => gs.GameId == wedstrijd.Id && gs.PlayerId == speler.Id).Select(gs => gs.Aantal180.GetValueOrDefault()).FirstOrDefault();
                return aantal180;
            }
        }
        public static double GetGemiddeldeFromGamescore(Game wedstrijd, Player speler)
        {
            using (var db = new DbDartsmanagerContext())
            {
                double gemiddelde = db.GameScores.Where(gs => gs.GameId == wedstrijd.Id && gs.PlayerId == speler.Id).Select(gs => gs.Gemiddelde.GetValueOrDefault()).FirstOrDefault();
                return gemiddelde;
            }
        }
        public static void UpdateGamescore(GameScore score)
        {
            try
            {
                using (var db = new DbDartsmanagerContext())
                {
                    var gamescore_gevonden = db.GameScores.FirstOrDefault(gs => gs.Id == score.Id);
                    if (gamescore_gevonden != null)
                    {
                        gamescore_gevonden.SetsWon = score.SetsWon;
                        gamescore_gevonden.LegsWon = score.LegsWon;
                        gamescore_gevonden.Aantal180 = score.Aantal180;
                        gamescore_gevonden.Gemiddelde = score.Gemiddelde;
                        db.SaveChanges();
                    }
                }
            }
            catch
            {
                throw new InvalidOperationException("Kon de score niet aanpassen. Controleer of de naam uniek is of de databaseverbinding juist is.");
            }
        }
        public static void UpdateLegsFromGamescore(Game wedstrijd, Player speler, int nieuwe_legs)
        {
            try
            {
                using (var db = new DbDartsmanagerContext())
                {
                    var gamescore = db.GameScores.FirstOrDefault(gs => gs.GameId == wedstrijd.Id && gs.PlayerId == speler.Id);
                    if (gamescore != null)
                    {
                        gamescore.LegsWon = nieuwe_legs;
                        db.SaveChanges();
                    }
                }
            }
            catch
            {
                throw new InvalidOperationException("Kon de score niet aanpassen. Controleer of de naam uniek is of de databaseverbinding juist is.");
            }
        }
        public static void UpdateSetFromLegs(Game wedstrijd)
        {
            using (var db = new DbDartsmanagerContext())
            {
                var scores = db.GameScores.Where(gs => gs.GameId == wedstrijd.Id).ToList();
 
                if (scores.Count != 2) 
                {
                    return;
                }
                var score_speler1 = scores[0];
                var score_speler2 = scores[1];

                int? legs1 = score_speler1.LegsWon;
                int? legs2 = score_speler2.LegsWon;
                if (legs1 != null && legs2 != null)
                {
                    // Reset sets
                    score_speler1.SetsWon = 0;
                    score_speler2.SetsWon = 0;

                    // Kijken wie er wint
                    if (legs1 >= 3 && legs1 > legs2)
                    {
                        score_speler1.SetsWon = 1;
                    }
                    else if (legs2 >= 3 && legs2 > legs1)
                    {
                        score_speler2.SetsWon = 1;
                    }
                    else if (legs1 > 0 && legs2 > 0 && legs2 ==  legs1)
                    {
                        MessageBox.Show("Beide spelers moeten een verschillende score halen!");
                    }

                    db.SaveChanges();
                }

                
            }
        }
        public static (Player winnaar, Player verliezer)? GetGameResult(Game wedstrijd)
        {
            var scores = GetAllScores(wedstrijd);
            if (scores != null && scores.Count >= 2)
            {
                if (scores[0].LegsWon != null && scores[1].LegsWon != null)
                {
                    if (scores[0].LegsWon > scores[1].LegsWon)
                    {
                        return (scores[0].Player, scores[1].Player);
                    }
                    else if (scores[1].LegsWon > scores[0].LegsWon)
                    {
                        return (scores[1].Player, scores[0].Player);
                    }
                }
            }
            return null;
        }

    }
}
