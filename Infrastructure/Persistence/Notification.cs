using System;
using System.Collections.Generic;

namespace Infrastructure.Persistence;

public partial class Notification
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public string Type { get; set; } = null!;

    public string Content { get; set; } = null!;

    public bool? IsRead { get; set; }

    public string? Path { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual User User { get; set; } = null!;
}
