using System;
using System.Collections.Generic;

namespace Domain.Entities;

public partial class Hashtag
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public DateTime? CreatedAt { get; set; }

    public virtual ICollection<HashtagPost> HashtagPosts { get; set; } = new List<HashtagPost>();
}
