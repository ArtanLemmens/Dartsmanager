using System;
using System.Collections.Generic;
using System.Text;

namespace Dartsmanager.Models
{
    public class TempRanking
    {
        public int PlayerId { get; set; }
        public int Groepsfase { get; set; }
        public int Gewonnen_Matchen { get; set; }
        public int Legs { get; set; }
        public int Aantal_180 { get; set; }
        public double Gemiddelde { get; set; }
    }
}
