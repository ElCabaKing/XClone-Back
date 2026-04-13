using System;
using System.Collections.Generic;

namespace Infrastructure.Persistence;

public partial class Role
{
    public int Id { get; set; }

    public string RoleName { get; set; } = null!;
}
