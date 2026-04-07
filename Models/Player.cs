using System;
using System.Collections.Generic;

namespace Dartsmanager.Models;

public partial class Player
{
    public int Id { get; set; }

    public string Naam { get; set; } = null!;

    public string Voornaam { get; set; } = null!;

    public int? IsDummy { get; set; }

    public string? Geboortedatum { get; set; }

    public string? Mail { get; set; }

    public int? AdresId { get; set; }

    public string? Telefoonnummer { get; set; }

    public int? Ranking { get; set; }

    public string? LidSinds { get; set; }

    public virtual Adress? Adres { get; set; }

    public virtual ICollection<Game> GamePlayer1s { get; set; } = new List<Game>();

    public virtual ICollection<Game> GamePlayer2s { get; set; } = new List<Game>();

    public IEnumerable<Game> Games => GamePlayer1s.Concat(GamePlayer2s).Distinct();

    public virtual ICollection<GameScore> GameScores { get; set; } = new List<GameScore>();

    public virtual ICollection<GroupPlayer> GroupPlayers { get; set; } = new List<GroupPlayer>();

    public virtual ICollection<Registration> Registrations { get; set; } = new List<Registration>();

    public virtual User? User { get; set; }

    public string VoornaamNaam
    {
        get
        {
            return $"{Voornaam} {Naam}";
        }
    }

    public string AdresVolledig
    {
        get
        {
            if (Adres == null)
            {
                return "";
            }
            if (Adres.Country == null)
            {
                return $"{Adres.Gemeente} {Adres.Huisnummer}{Adres.Toevoeging}, {Adres.Postcode} {Adres.Gemeente}";
            }
            return $"{Adres.Gemeente} {Adres.Huisnummer}{Adres.Toevoeging}, {Adres.Postcode} {Adres.Gemeente}, {Adres.Country.Naam}";
        }
    }
}
