using System;
using System.Collections.Generic;

namespace Dartsmanager.Models;

public partial class Country
{
    public int Id { get; set; }

    public string Naam { get; set; } = null!;

    public string Afkorting { get; set; } = null!;

    public virtual ICollection<Adress> Adresses { get; set; } = new List<Adress>();
}
