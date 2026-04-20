using System;
using System.Collections.Generic;
using Domain.Entities;
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

    public virtual DbSet<EmailTemplate> EmailTemplates { get; set; }

    public virtual DbSet<FollowList> FollowLists { get; set; }

    public virtual DbSet<Hashtag> Hashtags { get; set; }

    public virtual DbSet<HashtagPost> HashtagPosts { get; set; }

    public virtual DbSet<LikeList> LikeLists { get; set; }

    public virtual DbSet<Message> Messages { get; set; }

    public virtual DbSet<MuteList> MuteLists { get; set; }

    public virtual DbSet<Notification> Notifications { get; set; }

    public virtual DbSet<PasswordResetToken> PasswordResetTokens { get; set; }

    public virtual DbSet<Post> Posts { get; set; }

    public virtual DbSet<RefreshToken> RefreshTokens { get; set; }

    public virtual DbSet<Survey> Surveys { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserRole> UserRoles { get; set; }

    public virtual DbSet<UserStatus> UserStatuses { get; set; }

    public virtual DbSet<VerificationToken> VerificationTokens { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<BanList>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ban_list__3213E83F5F6F5654");

            entity.ToTable("ban_list");

            entity.HasIndex(e => new { e.UserId, e.BannedBy }, "UQ__ban_list__513803F6D732232A").IsUnique();

            entity.HasIndex(e => e.BannedBy, "idx_ban_list_banned_by");

            entity.HasIndex(e => e.UserId, "idx_ban_list_user_id");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
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
            entity.HasKey(e => e.Id).HasName("PK__chat_roo__3213E83F9D75C2F5");

            entity.ToTable("chat_room");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("created_at");

            entity.HasMany(d => d.Users).WithMany(p => p.Chats)
                .UsingEntity<Dictionary<string, object>>(
                    "ChatParticipant",
                    r => r.HasOne<User>().WithMany()
                        .HasForeignKey("UserId")
                        .HasConstraintName("FK__chat_part__user___6E2152BE"),
                    l => l.HasOne<ChatRoom>().WithMany()
                        .HasForeignKey("ChatId")
                        .HasConstraintName("FK__chat_part__chat___6D2D2E85"),
                    j =>
                    {
                        j.HasKey("ChatId", "UserId").HasName("PK__chat_par__169FE86742D73EA4");
                        j.ToTable("chat_participants");
                        j.HasIndex(new[] { "UserId" }, "idx_chat_participants_user_id");
                        j.IndexerProperty<Guid>("ChatId").HasColumnName("chat_id");
                        j.IndexerProperty<Guid>("UserId").HasColumnName("user_id");
                    });
        });

        modelBuilder.Entity<CommentLikeList>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__comment___3213E83FB984708E");

            entity.ToTable("comment_like_list");

            entity.HasIndex(e => new { e.UserId, e.CommentId }, "UQ__comment___D7C76066612610D2").IsUnique();

            entity.HasIndex(e => e.CommentId, "idx_comment_like_list_comment_id");

            entity.HasIndex(e => e.UserId, "idx_comment_like_list_user_id");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.CommentId).HasColumnName("comment_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("created_at");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Comment).WithMany(p => p.CommentLikeLists)
                .HasForeignKey(d => d.CommentId)
                .HasConstraintName("FK__comment_l__comme__5DEAEAF5");

            entity.HasOne(d => d.User).WithMany(p => p.CommentLikeLists)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__comment_l__user___5CF6C6BC");
        });

        modelBuilder.Entity<CommentList>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__comment___3213E83F7C954731");

            entity.ToTable("comment_list");

            entity.HasIndex(e => e.CommentTo, "idx_comment_list_comment_to");

            entity.HasIndex(e => e.PostId, "idx_comment_list_post_id");

            entity.HasIndex(e => new { e.PostId, e.UserId }, "idx_comment_list_post_user");

            entity.HasIndex(e => e.UserId, "idx_comment_list_user_id");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
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
                .HasConstraintName("FK__comment_l__comme__5832119F");

            entity.HasOne(d => d.Post).WithMany(p => p.CommentLists)
                .HasForeignKey(d => d.PostId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__comment_l__post___573DED66");

            entity.HasOne(d => d.User).WithMany(p => p.CommentLists)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__comment_l__user___5649C92D");
        });

        modelBuilder.Entity<EmailTemplate>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__email_te__3213E83FA2E42DEB");

            entity.ToTable("email_templates");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Body).HasColumnName("body");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
            entity.Property(e => e.Subject)
                .HasMaxLength(255)
                .HasColumnName("subject");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("updated_at");
        });

        modelBuilder.Entity<FollowList>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__follow_l__3213E83FCA707842");

            entity.ToTable("follow_list");

            entity.HasIndex(e => new { e.FollowerId, e.FollowingId }, "UQ__follow_l__CAC186A686D55DB0").IsUnique();

            entity.HasIndex(e => e.FollowerId, "idx_follow_list_follower_id");

            entity.HasIndex(e => e.FollowingId, "idx_follow_list_following_id");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("created_at");
            entity.Property(e => e.FollowerId).HasColumnName("follower_id");
            entity.Property(e => e.FollowingId).HasColumnName("following_id");

            entity.HasOne(d => d.Follower).WithMany(p => p.FollowListFollowers)
                .HasForeignKey(d => d.FollowerId)
                .HasConstraintName("FK__follow_li__follo__4BCC3ABA");

            entity.HasOne(d => d.Following).WithMany(p => p.FollowListFollowings)
                .HasForeignKey(d => d.FollowingId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__follow_li__follo__4CC05EF3");
        });

        modelBuilder.Entity<Hashtag>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__hashtags__3213E83F5839FE94");

            entity.ToTable("hashtags");

            entity.HasIndex(e => e.Name, "UQ__hashtags__72E12F1B5F3D6311").IsUnique();

            entity.HasIndex(e => e.Name, "idx_hashtags_name");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
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
            entity.HasKey(e => e.Id).HasName("PK__hashtag___3213E83F135A1C6A");

            entity.ToTable("hashtag_post");

            entity.HasIndex(e => new { e.HashtagId, e.PostId }, "UQ__hashtag___8671FC9B0272C0D9").IsUnique();

            entity.HasIndex(e => e.HashtagId, "idx_hashtag_post_hashtag_id");

            entity.HasIndex(e => e.PostId, "idx_hashtag_post_post_id");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("created_at");
            entity.Property(e => e.HashtagId).HasColumnName("hashtag_id");
            entity.Property(e => e.PostId).HasColumnName("post_id");

            entity.HasOne(d => d.Hashtag).WithMany(p => p.HashtagPosts)
                .HasForeignKey(d => d.HashtagId)
                .HasConstraintName("FK__hashtag_p__hasht__668030F6");

            entity.HasOne(d => d.Post).WithMany(p => p.HashtagPosts)
                .HasForeignKey(d => d.PostId)
                .HasConstraintName("FK__hashtag_p__post___6774552F");
        });

        modelBuilder.Entity<LikeList>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__like_lis__3213E83F2795A911");

            entity.ToTable("like_list");

            entity.HasIndex(e => new { e.UserId, e.PostId }, "UQ__like_lis__CA534F785E4827FB").IsUnique();

            entity.HasIndex(e => e.PostId, "idx_like_list_post_id");

            entity.HasIndex(e => e.UserId, "idx_like_list_user_id");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("created_at");
            entity.Property(e => e.PostId).HasColumnName("post_id");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Post).WithMany(p => p.LikeLists)
                .HasForeignKey(d => d.PostId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__like_list__post___52793849");

            entity.HasOne(d => d.User).WithMany(p => p.LikeLists)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__like_list__user___51851410");
        });

        modelBuilder.Entity<Message>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__messages__3213E83FC47CE4A9");

            entity.ToTable("messages");

            entity.HasIndex(e => new { e.ChatRoomId, e.CreatedAt }, "idx_messages_chat_created").IsDescending(false, true);

            entity.HasIndex(e => e.ChatRoomId, "idx_messages_chat_room_id");

            entity.HasIndex(e => e.IsRead, "idx_messages_is_read");

            entity.HasIndex(e => e.SenderId, "idx_messages_sender_id");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
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
                .HasConstraintName("FK__messages__chat_r__72E607DB");

            entity.HasOne(d => d.Sender).WithMany(p => p.Messages)
                .HasForeignKey(d => d.SenderId)
                .HasConstraintName("FK__messages__sender__73DA2C14");
        });

        modelBuilder.Entity<MuteList>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__mute_lis__3213E83F66C7781B");

            entity.ToTable("mute_list");

            entity.HasIndex(e => new { e.UserId, e.MutedBy }, "UQ__mute_lis__C6E2D08C098F77B4").IsUnique();

            entity.HasIndex(e => e.MutedBy, "idx_mute_list_muted_by");

            entity.HasIndex(e => e.UserId, "idx_mute_list_user_id");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("created_at");
            entity.Property(e => e.MutedBy).HasColumnName("muted_by");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.MutedByNavigation).WithMany(p => p.MuteListMutedByNavigations)
                .HasForeignKey(d => d.MutedBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__mute_list__muted__46136164");

            entity.HasOne(d => d.User).WithMany(p => p.MuteListUsers)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__mute_list__user___451F3D2B");
        });

        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__notifica__3213E83FA1D07D61");

            entity.ToTable("notifications");

            entity.HasIndex(e => e.CreatedAt, "idx_notifications_created_at").IsDescending();

            entity.HasIndex(e => e.IsRead, "idx_notifications_is_read");

            entity.HasIndex(e => e.UserId, "idx_notifications_user_id");

            entity.HasIndex(e => new { e.UserId, e.IsRead }, "idx_notifications_user_read");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
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
                .HasConstraintName("FK__notificat__user___789EE131");
        });

        modelBuilder.Entity<PasswordResetToken>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Password__3213E83FEEACDB72");

            entity.ToTable("PasswordResetToken");

            entity.HasIndex(e => e.ResetToken, "UQ__Password__25F405EB42CC665A").IsUnique();

            entity.HasIndex(e => e.UserId, "idx_password_reset_token_user_id");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("created_at");
            entity.Property(e => e.ExpiresAt).HasColumnName("expires_at");
            entity.Property(e => e.ResetToken)
                .HasMaxLength(255)
                .HasColumnName("reset_token");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.User).WithMany(p => p.PasswordResetTokens)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PasswordResetToken_User");
        });

        modelBuilder.Entity<Post>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__post__3213E83F0D0AB2AE");

            entity.ToTable("post");

            entity.HasIndex(e => e.CreatedAt, "idx_post_created_at").IsDescending();

            entity.HasIndex(e => e.ReplyTo, "idx_post_reply_to");

            entity.HasIndex(e => e.RepostOf, "idx_post_repost_of");

            entity.HasIndex(e => new { e.UserId, e.CreatedAt }, "idx_post_user_created").IsDescending(false, true);

            entity.HasIndex(e => e.UserId, "idx_post_user_id");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
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
                .HasConstraintName("FK__post__reply_to__33008CF0");

            entity.HasOne(d => d.RepostOfNavigation).WithMany(p => p.InverseRepostOfNavigation)
                .HasForeignKey(d => d.RepostOf)
                .HasConstraintName("FK__post__repost_of__33F4B129");

            entity.HasOne(d => d.User).WithMany(p => p.Posts)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__post__user_id__320C68B7");
        });

        modelBuilder.Entity<RefreshToken>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__RefreshT__3213E83F98720F7A");

            entity.ToTable("RefreshToken");

            entity.HasIndex(e => e.RefreshToken1, "UQ__RefreshT__7FB69BAD942EFD04").IsUnique();

            entity.HasIndex(e => e.UserId, "idx_refresh_token_user_id");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("created_at");
            entity.Property(e => e.ExpiresAt).HasColumnName("expires_at");
            entity.Property(e => e.RefreshToken1)
                .HasMaxLength(255)
                .HasColumnName("refresh_token");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.User).WithMany(p => p.RefreshTokens)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_RefreshToken_User");
        });

        modelBuilder.Entity<Survey>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__survey__3213E83F243B7C4B");

            entity.ToTable("survey");

            entity.HasIndex(e => e.PostId, "idx_survey_post_id");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("created_at");
            entity.Property(e => e.Options).HasColumnName("options");
            entity.Property(e => e.PostId).HasColumnName("post_id");
            entity.Property(e => e.Question).HasColumnName("question");

            entity.HasOne(d => d.Post).WithMany(p => p.Surveys)
                .HasForeignKey(d => d.PostId)
                .HasConstraintName("FK__survey__post_id__37C5420D");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__user__3213E83F993060CB");

            entity.ToTable("user");

            entity.HasIndex(e => e.Email, "UQ__user__AB6E61649D47CD66").IsUnique();

            entity.HasIndex(e => e.Username, "UQ__user__F3DBC572444D289A").IsUnique();

            entity.HasIndex(e => e.CreatedAt, "idx_user_created_at");

            entity.HasIndex(e => e.RoleId, "idx_user_role_id");

            entity.HasIndex(e => e.StatusId, "idx_user_status_id");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
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
            entity.Property(e => e.LastName)
                .HasMaxLength(255)
                .HasColumnName("last_name");
            entity.Property(e => e.PasswordHash)
                .HasMaxLength(255)
                .HasColumnName("password_hash");
            entity.Property(e => e.ProfilePictureUrl)
                .HasMaxLength(255)
                .HasColumnName("profile_picture_url");
            entity.Property(e => e.RoleId)
                .HasDefaultValue((byte)2)
                .HasColumnName("role_id");
            entity.Property(e => e.StatusId)
                .HasDefaultValue((byte)1)
                .HasColumnName("status_id");
            entity.Property(e => e.Username)
                .HasMaxLength(255)
                .HasColumnName("username");

            entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .HasForeignKey(d => d.RoleId)
                .HasConstraintName("FK_User_Role");

            entity.HasOne(d => d.Status).WithMany(p => p.Users)
                .HasForeignKey(d => d.StatusId)
                .HasConstraintName("FK_User_Status");
        });

        modelBuilder.Entity<UserRole>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__user_rol__3213E83FD532C07C");

            entity.ToTable("user_role");

            entity.HasIndex(e => e.Name, "UQ__user_rol__72E12F1B9A27D147").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Description)
                .HasMaxLength(255)
                .HasColumnName("description");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");
        });

        modelBuilder.Entity<UserStatus>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__user_sta__3213E83FEC6ACA4A");

            entity.ToTable("user_status");

            entity.HasIndex(e => e.Name, "UQ__user_sta__72E12F1B7D554900").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Description)
                .HasMaxLength(255)
                .HasColumnName("description");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");
        });

        modelBuilder.Entity<VerificationToken>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Verifica__3213E83FC5A6C823");

            entity.ToTable("VerificationToken");

            entity.HasIndex(e => e.AccessToken, "UQ__Verifica__4EDC6DFCFCA90A8B").IsUnique();

            entity.HasIndex(e => e.UserId, "idx_verification_token_user_id");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.AccessToken)
                .HasMaxLength(255)
                .HasColumnName("access_token");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("created_at");
            entity.Property(e => e.ExpiresAt).HasColumnName("expires_at");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.User).WithMany(p => p.VerificationTokens)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_VerificationToken_User");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
