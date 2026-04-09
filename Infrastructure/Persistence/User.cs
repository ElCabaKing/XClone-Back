using System;
using System.Collections.Generic;

namespace Infrastructure.Persistence;

public partial class User
{
    public Guid Id { get; set; }

    public string Username { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public bool IsVerified { get; set; }

    public byte StatusId { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string? ProfilePictureUrl { get; set; }

    public virtual ICollection<BanList> BanListBannedByNavigations { get; set; } = new List<BanList>();

    public virtual ICollection<BanList> BanListUsers { get; set; } = new List<BanList>();

    public virtual ICollection<CommentLikeList> CommentLikeLists { get; set; } = new List<CommentLikeList>();

    public virtual ICollection<CommentList> CommentLists { get; set; } = new List<CommentList>();

    public virtual ICollection<FollowList> FollowListFollowers { get; set; } = new List<FollowList>();

    public virtual ICollection<FollowList> FollowListFollowings { get; set; } = new List<FollowList>();

    public virtual ICollection<LikeList> LikeLists { get; set; } = new List<LikeList>();

    public virtual ICollection<Message> Messages { get; set; } = new List<Message>();

    public virtual ICollection<MuteList> MuteListMutedByNavigations { get; set; } = new List<MuteList>();

    public virtual ICollection<MuteList> MuteListUsers { get; set; } = new List<MuteList>();

    public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();

    public virtual ICollection<Post> Posts { get; set; } = new List<Post>();

    public virtual UserStatus Status { get; set; } = null!;

    public virtual ICollection<ChatRoom> Chats { get; set; } = new List<ChatRoom>();
}
