using System;
using System.Collections.Generic;

namespace Dartsmanager.Models;

public partial class Registration
{
    public int Id { get; set; }

    public int TournamentId { get; set; }

    public int PlayerId { get; set; }

    public string? Datum { get; set; }

    public virtual Player Player { get; set; } = null!;

    public virtual Tournament Tournament { get; set; } = null!;
}
