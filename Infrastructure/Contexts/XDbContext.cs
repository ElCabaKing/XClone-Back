using System;
using System.Collections.Generic;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Contexts;

public partial class XDbContext : DbContext
{
    public XDbContext()
    {
    }

    public XDbContext(DbContextOptions<XDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<BanList> BanLists { get; set; }

    public virtual DbSet<ChatRoom> ChatRooms { get; set; }

    public virtual DbSet<CommentLikeList> CommentLikeLists { get; set; }

    public virtual DbSet<CommentList> CommentLists { get; set; }

    public virtual DbSet<FollowList> FollowLists { get; set; }

    public virtual DbSet<Hashtag> Hashtags { get; set; }

    public virtual DbSet<HashtagPost> HashtagPosts { get; set; }

    public virtual DbSet<LikeList> LikeLists { get; set; }

    public virtual DbSet<Message> Messages { get; set; }

    public virtual DbSet<MuteList> MuteLists { get; set; }

    public virtual DbSet<Notification> Notifications { get; set; }

    public virtual DbSet<Post> Posts { get; set; }

    public virtual DbSet<Survey> Surveys { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserStatus> UserStatuses { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=localhost,1433;User=sa;Password=YourStrong@Password123;TrustServerCertificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<BanList>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ban_list__3213E83FA39CD2A8");

            entity.ToTable("ban_list");

            entity.HasIndex(e => new { e.UserId, e.BannedBy }, "UQ__ban_list__513803F657F356D7").IsUnique();

            entity.Property(e => e.Id)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("id");
            entity.Property(e => e.BannedBy).HasColumnName("banned_by");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("created_at");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.BannedByNavigation).WithMany(p => p.BanListBannedByNavigations)
                .HasForeignKey(d => d.BannedBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ban_list__banned__3F6663D5");

            entity.HasOne(d => d.User).WithMany(p => p.BanListUsers)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__ban_list__user_i__3E723F9C");
        });

        modelBuilder.Entity<ChatRoom>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__chat_roo__3213E83F3F570FB1");

            entity.ToTable("chat_room");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("created_at");

            entity.HasMany(d => d.Users).WithMany(p => p.Chats)
                .UsingEntity<Dictionary<string, object>>(
                    "ChatParticipant",
                    r => r.HasOne<User>().WithMany()
                        .HasForeignKey("UserId")
                        .HasConstraintName("FK__chat_part__user___75C27486"),
                    l => l.HasOne<ChatRoom>().WithMany()
                        .HasForeignKey("ChatId")
                        .HasConstraintName("FK__chat_part__chat___74CE504D"),
                    j =>
                    {
                        j.HasKey("ChatId", "UserId").HasName("PK__chat_par__169FE867D2104E01");
                        j.ToTable("chat_participants");
                        j.IndexerProperty<Guid>("ChatId").HasColumnName("chat_id");
                        j.IndexerProperty<Guid>("UserId").HasColumnName("user_id");
                    });
        });

        modelBuilder.Entity<CommentLikeList>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__comment___3213E83F1AA55E09");

            entity.ToTable("comment_like_list");

            entity.HasIndex(e => new { e.UserId, e.CommentId }, "UQ__comment___D7C7606675C664EF").IsUnique();

            entity.Property(e => e.Id)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("id");
            entity.Property(e => e.CommentId).HasColumnName("comment_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("created_at");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Comment).WithMany(p => p.CommentLikeLists)
                .HasForeignKey(d => d.CommentId)
                .HasConstraintName("FK__comment_l__comme__62AFA012");

            entity.HasOne(d => d.User).WithMany(p => p.CommentLikeLists)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__comment_l__user___61BB7BD9");
        });

        modelBuilder.Entity<CommentList>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__comment___3213E83FA3D5018B");

            entity.ToTable("comment_list");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("id");
            entity.Property(e => e.CommentTo).HasColumnName("comment_to");
            entity.Property(e => e.Content).HasColumnName("content");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("created_at");
            entity.Property(e => e.MediaType)
                .HasMaxLength(50)
                .HasColumnName("media_type");
            entity.Property(e => e.MediaUrl).HasColumnName("media_url");
            entity.Property(e => e.PostId).HasColumnName("post_id");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.CommentToNavigation).WithMany(p => p.InverseCommentToNavigation)
                .HasForeignKey(d => d.CommentTo)
                .HasConstraintName("FK__comment_l__comme__5C02A283");

            entity.HasOne(d => d.Post).WithMany(p => p.CommentLists)
                .HasForeignKey(d => d.PostId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__comment_l__post___5B0E7E4A");

            entity.HasOne(d => d.User).WithMany(p => p.CommentLists)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__comment_l__user___5A1A5A11");
        });

        modelBuilder.Entity<FollowList>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__follow_l__3213E83F099FB849");

            entity.ToTable("follow_list");

            entity.HasIndex(e => new { e.FollowerId, e.FollowingId }, "UQ__follow_l__CAC186A68C4303FF").IsUnique();

            entity.Property(e => e.Id)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("created_at");
            entity.Property(e => e.FollowerId).HasColumnName("follower_id");
            entity.Property(e => e.FollowingId).HasColumnName("following_id");

            entity.HasOne(d => d.Follower).WithMany(p => p.FollowListFollowers)
                .HasForeignKey(d => d.FollowerId)
                .HasConstraintName("FK__follow_li__follo__4DB4832C");

            entity.HasOne(d => d.Following).WithMany(p => p.FollowListFollowings)
                .HasForeignKey(d => d.FollowingId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__follow_li__follo__4EA8A765");
        });

        modelBuilder.Entity<Hashtag>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__hashtags__3213E83FB97D9AFD");

            entity.ToTable("hashtags");

            entity.HasIndex(e => e.Name, "UQ__hashtags__72E12F1BB1C8E70C").IsUnique();

            entity.Property(e => e.Id)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("created_at");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
        });

        modelBuilder.Entity<HashtagPost>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__hashtag___3213E83FC1FE28BD");

            entity.ToTable("hashtag_post");

            entity.HasIndex(e => new { e.HashtagId, e.PostId }, "UQ__hashtag___8671FC9B0655351F").IsUnique();

            entity.Property(e => e.Id)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("created_at");
            entity.Property(e => e.HashtagId).HasColumnName("hashtag_id");
            entity.Property(e => e.PostId).HasColumnName("post_id");

            entity.HasOne(d => d.Hashtag).WithMany(p => p.HashtagPosts)
                .HasForeignKey(d => d.HashtagId)
                .HasConstraintName("FK__hashtag_p__hasht__6D2D2E85");

            entity.HasOne(d => d.Post).WithMany(p => p.HashtagPosts)
                .HasForeignKey(d => d.PostId)
                .HasConstraintName("FK__hashtag_p__post___6E2152BE");
        });

        modelBuilder.Entity<LikeList>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__like_lis__3213E83FD283CB03");

            entity.ToTable("like_list");

            entity.HasIndex(e => new { e.UserId, e.PostId }, "UQ__like_lis__CA534F78137ED219").IsUnique();

            entity.Property(e => e.Id)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("created_at");
            entity.Property(e => e.PostId).HasColumnName("post_id");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Post).WithMany(p => p.LikeLists)
                .HasForeignKey(d => d.PostId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__like_list__post___5555A4F4");

            entity.HasOne(d => d.User).WithMany(p => p.LikeLists)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__like_list__user___546180BB");
        });

        modelBuilder.Entity<Message>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__messages__3213E83FAB3DBAA2");

            entity.ToTable("messages");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("id");
            entity.Property(e => e.ChatRoomId).HasColumnName("chat_room_id");
            entity.Property(e => e.Content).HasColumnName("content");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("created_at");
            entity.Property(e => e.IsRead)
                .HasDefaultValue(false)
                .HasColumnName("is_read");
            entity.Property(e => e.SenderId).HasColumnName("sender_id");

            entity.HasOne(d => d.ChatRoom).WithMany(p => p.Messages)
                .HasForeignKey(d => d.ChatRoomId)
                .HasConstraintName("FK__messages__chat_r__7B7B4DDC");

            entity.HasOne(d => d.Sender).WithMany(p => p.Messages)
                .HasForeignKey(d => d.SenderId)
                .HasConstraintName("FK__messages__sender__7C6F7215");
        });

        modelBuilder.Entity<MuteList>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__mute_lis__3213E83F04909C1C");

            entity.ToTable("mute_list");

            entity.HasIndex(e => new { e.UserId, e.MutedBy }, "UQ__mute_lis__C6E2D08C985847E9").IsUnique();

            entity.Property(e => e.Id)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("created_at");
            entity.Property(e => e.MutedBy).HasColumnName("muted_by");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.MutedByNavigation).WithMany(p => p.MuteListMutedByNavigations)
                .HasForeignKey(d => d.MutedBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__mute_list__muted__4707859D");

            entity.HasOne(d => d.User).WithMany(p => p.MuteListUsers)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__mute_list__user___46136164");
        });

        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__notifica__3213E83F3382515C");

            entity.ToTable("notifications");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("id");
            entity.Property(e => e.Content).HasColumnName("content");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("created_at");
            entity.Property(e => e.IsRead)
                .HasDefaultValue(false)
                .HasColumnName("is_read");
            entity.Property(e => e.Path)
                .HasMaxLength(255)
                .HasColumnName("path");
            entity.Property(e => e.Type)
                .HasMaxLength(50)
                .HasColumnName("type");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.User).WithMany(p => p.Notifications)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__notificat__user___02284B6B");
        });

        modelBuilder.Entity<Post>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__post__3213E83FCFDE2AD4");

            entity.ToTable("post");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("id");
            entity.Property(e => e.Content).HasColumnName("content");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("created_at");
            entity.Property(e => e.IsOutstanding)
                .HasDefaultValue(false)
                .HasColumnName("is_outstanding");
            entity.Property(e => e.IsSensitive)
                .HasDefaultValue(false)
                .HasColumnName("is_sensitive");
            entity.Property(e => e.MediaType)
                .HasMaxLength(50)
                .HasColumnName("media_type");
            entity.Property(e => e.MediaUrl)
                .HasMaxLength(255)
                .HasColumnName("media_url");
            entity.Property(e => e.ReplyTo).HasColumnName("reply_to");
            entity.Property(e => e.RepostOf).HasColumnName("repost_of");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.ReplyToNavigation).WithMany(p => p.InverseReplyToNavigation)
                .HasForeignKey(d => d.ReplyTo)
                .HasConstraintName("FK__post__reply_to__3118447E");

            entity.HasOne(d => d.RepostOfNavigation).WithMany(p => p.InverseRepostOfNavigation)
                .HasForeignKey(d => d.RepostOf)
                .HasConstraintName("FK__post__repost_of__320C68B7");

            entity.HasOne(d => d.User).WithMany(p => p.Posts)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__post__user_id__30242045");
        });

        modelBuilder.Entity<Survey>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__survey__3213E83F7F9FD25B");

            entity.ToTable("survey");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("created_at");
            entity.Property(e => e.Options).HasColumnName("options");
            entity.Property(e => e.PostId).HasColumnName("post_id");
            entity.Property(e => e.Question).HasColumnName("question");

            entity.HasOne(d => d.Post).WithMany(p => p.Surveys)
                .HasForeignKey(d => d.PostId)
                .HasConstraintName("FK__survey__post_id__36D11DD4");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__user__3213E83F41E9CC29");

            entity.ToTable("user");

            entity.HasIndex(e => e.Email, "UQ__user__AB6E6164444CE62E").IsUnique();

            entity.HasIndex(e => e.Username, "UQ__user__F3DBC572058C3B64").IsUnique();

            entity.Property(e => e.Id)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("created_at");
            entity.Property(e => e.Email)
                .HasMaxLength(255)
                .HasColumnName("email");
            entity.Property(e => e.FirstName)
                .HasMaxLength(255)
                .HasColumnName("first_name");
            entity.Property(e => e.IsVerified).HasColumnName("is_verified");
            entity.Property(e => e.LastName)
                .HasMaxLength(255)
                .HasColumnName("last_name");
            entity.Property(e => e.PasswordHash)
                .HasMaxLength(255)
                .HasColumnName("password_hash");
            entity.Property(e => e.ProfilePictureUrl)
                .HasMaxLength(255)
                .HasColumnName("profile_picture_url");
            entity.Property(e => e.StatusId)
                .HasDefaultValue((byte)1)
                .HasColumnName("status_id");
            entity.Property(e => e.Username)
                .HasMaxLength(255)
                .HasColumnName("username");

            entity.HasOne(d => d.Status).WithMany(p => p.Users)
                .HasForeignKey(d => d.StatusId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_User_Status");
        });

        modelBuilder.Entity<UserStatus>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__user_sta__3213E83F5911EDDE");

            entity.ToTable("user_status");

            entity.HasIndex(e => e.Name, "UQ__user_sta__72E12F1B15642442").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Description)
                .HasMaxLength(255)
                .HasColumnName("description");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
