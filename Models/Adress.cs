using System;
using System.Collections.Generic;

namespace Dartsmanager.Models;

public partial class Adress
{
    public int Id { get; set; }

    public string Straat { get; set; } = null!;

    public int Huisnummer { get; set; }

    public string? Toevoeging { get; set; }

    public string? Postcode { get; set; }

    public string? Gemeente { get; set; }

    public int? CountryId { get; set; }

    public virtual Country? Country { get; set; }

    public virtual ICollection<Player> Players { get; set; } = new List<Player>();

    public virtual ICollection<Tournament> Tournaments { get; set; } = new List<Tournament>();

    public string AdresVolledig
    {
        get
        {
            if (Straat == "-- Selecteer adres --")
            {
                return Straat;
            }
            string adres = $"{Straat} {Huisnummer}{Toevoeging}, {Postcode} {Gemeente}";
            if (Country != null)
            {
                adres += $", {Country.Naam}";
            }
            return adres;
        }
    }
}
