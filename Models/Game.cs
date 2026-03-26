using System;
using System.Collections.Generic;

namespace Dartsmanager.Models;

public partial class Game
{
    public int Id { get; set; }

    public int? TournamentId { get; set; }

    public int? Ronde { get; set; }

    public int Player1Id { get; set; }

    public int Player2Id { get; set; }

    public int? GroupId { get; set; }

    public virtual ICollection<GameScore> GameScores { get; set; } = new List<GameScore>();

    public virtual Group? Group { get; set; }

    public virtual Player Player1 { get; set; } = null!;

    public virtual Player Player2 { get; set; } = null!;

    public virtual Tournament? Tournament { get; set; }
}
