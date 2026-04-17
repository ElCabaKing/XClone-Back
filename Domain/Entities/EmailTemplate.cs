using System;
using System.Collections.Generic;

namespace Domain.Entities;

public partial class EmailTemplate
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Subject { get; set; } = null!;

    public string Body { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }
}
