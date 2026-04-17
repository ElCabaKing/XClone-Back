using System;
using System.Collections.Generic;

namespace Domain.Entities;

public partial class Survey
{
    public Guid Id { get; set; }

    public string Question { get; set; } = null!;

    public string Options { get; set; } = null!;

    public Guid PostId { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual Post Post { get; set; } = null!;
}
