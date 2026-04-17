using System;
using System.Collections.Generic;

namespace Domain.Entities;

public partial class ChatRoom
{
    public Guid Id { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual ICollection<Message> Messages { get; set; } = new List<Message>();

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
