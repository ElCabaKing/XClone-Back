using System;
using System.Collections.Generic;

namespace Infrastructure.Persistence;

public partial class Post
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public string Content { get; set; } = null!;

    public DateTime? CreatedAt { get; set; }

    public string? MediaUrl { get; set; }

    public string? MediaType { get; set; }

    public bool? IsSensitive { get; set; }

    public bool? IsOutstanding { get; set; }

    public Guid? ReplyTo { get; set; }

    public Guid? RepostOf { get; set; }

    public virtual ICollection<CommentList> CommentLists { get; set; } = new List<CommentList>();

    public virtual ICollection<HashtagPost> HashtagPosts { get; set; } = new List<HashtagPost>();

    public virtual ICollection<Post> InverseReplyToNavigation { get; set; } = new List<Post>();

    public virtual ICollection<Post> InverseRepostOfNavigation { get; set; } = new List<Post>();

    public virtual ICollection<LikeList> LikeLists { get; set; } = new List<LikeList>();

    public virtual Post? ReplyToNavigation { get; set; }

    public virtual Post? RepostOfNavigation { get; set; }

    public virtual ICollection<Survey> Surveys { get; set; } = new List<Survey>();

    public virtual User User { get; set; } = null!;
}
