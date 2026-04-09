using System;
using System.Collections.Generic;

namespace Infrastructure.Persistence;

public partial class BanList
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public Guid BannedBy { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual User BannedByNavigation { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
