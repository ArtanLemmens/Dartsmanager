using System;
using System.Collections.Generic;

namespace Dartsmanager.Models;

public partial class GroupPlayer
{
    public int Id { get; set; }

    public int GroupId { get; set; }

    public int PlayerId { get; set; }

    public virtual Group Group { get; set; } = null!;

    public virtual Player Player { get; set; } = null!;
}
