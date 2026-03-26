using System;
using System.Collections.Generic;

namespace Dartsmanager.Models;

public partial class Status
{
    public int Id { get; set; }

    public string Naam { get; set; } = null!;

    public virtual ICollection<Tournament> Tournaments { get; set; } = new List<Tournament>();
}
