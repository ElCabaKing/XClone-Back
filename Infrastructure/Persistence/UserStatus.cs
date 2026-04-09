using System;
using System.Collections.Generic;

namespace Infrastructure.Persistence;

public partial class UserStatus
{
    public byte Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
