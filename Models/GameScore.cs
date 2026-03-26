using System;
using System.Collections.Generic;

namespace Dartsmanager.Models;

public partial class GameScore
{
    public int Id { get; set; }

    public int GameId { get; set; }

    public int PlayerId { get; set; }

    public int? SetsWon { get; set; }

    public int? LegsWon { get; set; }

    public int? Aantal180 { get; set; }

    public double? Gemiddelde { get; set; }

    public virtual Game Game { get; set; } = null!;

    public virtual Player Player { get; set; } = null!;
}
