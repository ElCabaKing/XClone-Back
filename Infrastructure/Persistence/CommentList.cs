using System;
using System.Collections.Generic;

namespace Infrastructure.Persistence;

public partial class CommentList
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public Guid PostId { get; set; }

    public string Content { get; set; } = null!;

    public string? MediaUrl { get; set; }

    public string? MediaType { get; set; }

    public Guid? CommentTo { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual ICollection<CommentLikeList> CommentLikeLists { get; set; } = new List<CommentLikeList>();

    public virtual CommentList? CommentToNavigation { get; set; }

    public virtual ICollection<CommentList> InverseCommentToNavigation { get; set; } = new List<CommentList>();

    public virtual Post Post { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
