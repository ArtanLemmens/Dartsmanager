using System;
using System.Collections.Generic;

namespace Dartsmanager.Models;

public partial class Tournament
{
    public int Id { get; set; }

    public string Naam { get; set; } = null!;

    public int? AdresId { get; set; }

    public int? Jaargang { get; set; }

    public string? Datum { get; set; }

    public int? StatusId { get; set; }

    public int? MaxInschrijvingen { get; set; }

    public int? AantalRondes { get; set; }

    public int? ActieveRonde { get; set; }

    public string? ExtraInfo { get; set; }

    public virtual Adress? Adres { get; set; }

    public virtual ICollection<Game> Games { get; set; } = new List<Game>();

    public virtual ICollection<Group> Groups { get; set; } = new List<Group>();

    public virtual ICollection<Registration> Registrations { get; set; } = new List<Registration>();

    public virtual Status? Status { get; set; }
}
