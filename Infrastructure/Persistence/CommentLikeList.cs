using System;
using System.Collections.Generic;

namespace Infrastructure.Persistence;

public partial class CommentLikeList
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public Guid CommentId { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual CommentList Comment { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
