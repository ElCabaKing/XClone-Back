using System;
using System.Collections.Generic;

namespace Domain.Entities;

public partial class MuteList
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public Guid MutedBy { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual User MutedByNavigation { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
