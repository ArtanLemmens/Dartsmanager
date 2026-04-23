using System;
using System.Collections.Generic;
using System.Text;

namespace Dartsmanager.Models
{
    public class GroupPlayerInfo
    {
        public Player Speler { get; set; } = null!;
        public int GroepSets { get; set; }
        public int GroepLegs { get; set; }
        public int Groep180 { get; set; }
        public double GroepGemiddelde { get; set; }
    }
}
