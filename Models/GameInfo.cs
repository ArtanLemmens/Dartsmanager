using System;
using System.Collections.Generic;
using System.Text;

namespace Dartsmanager.Models
{
    public class GameInfo
    {
        public Game Wedstrijd { get; set; } = null!;
        public Player Speler1 { get; set; } = null!;
        public Player Speler2 { get; set; } = null!;
        public int Legs1 { get; set; }
        public int Legs2 { get; set; }
        public int Number_180_1 { get; set; }
        public int Number_180_2 { get; set; }
        public double Gemiddelde1 { get; set; }
        public double Gemiddelde2 { get; set; }
    }
}
