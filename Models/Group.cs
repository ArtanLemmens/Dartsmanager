using System;
using System.Collections.Generic;

namespace Dartsmanager.Models;

public partial class Group
{
    public int Id { get; set; }

    public int TournamentId { get; set; }

    public int GroepNummer { get; set; }

    public virtual ICollection<Game> Games { get; set; } = new List<Game>();

    public virtual ICollection<GroupPlayer> GroupPlayers { get; set; } = new List<GroupPlayer>();

    public virtual Tournament Tournament { get; set; } = null!;
}
