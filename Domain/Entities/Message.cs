using System;
using System.Collections.Generic;

namespace Domain.Entities;

public partial class Message
{
    public Guid Id { get; set; }

    public Guid ChatRoomId { get; set; }

    public Guid SenderId { get; set; }

    public string Content { get; set; } = null!;

    public DateTime? CreatedAt { get; set; }

    public bool? IsRead { get; set; }

    public virtual ChatRoom ChatRoom { get; set; } = null!;

    public virtual User Sender { get; set; } = null!;
}
