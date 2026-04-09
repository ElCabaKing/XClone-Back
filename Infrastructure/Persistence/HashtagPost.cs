using System;
using System.Collections.Generic;

namespace Infrastructure.Persistence;

public partial class HashtagPost
{
    public Guid Id { get; set; }

    public Guid HashtagId { get; set; }

    public Guid PostId { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual Hashtag Hashtag { get; set; } = null!;

    public virtual Post Post { get; set; } = null!;
}
