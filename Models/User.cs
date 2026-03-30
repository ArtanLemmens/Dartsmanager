using System;
using System.Collections.Generic;

namespace Dartsmanager.Models;

public partial class User
{
    public int Id { get; set; }

    public string Username { get; set; } = null!;

    public string WachtwoordHash { get; set; } = null!;

    public bool IsAdmin { get; set; }

    public int? PlayerId { get; set; }

    public bool? PlayerIdBevestigd { get; set; }

    public virtual Player? Player { get; set; }
}
