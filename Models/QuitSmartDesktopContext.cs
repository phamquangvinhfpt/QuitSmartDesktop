using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace QuitSmartApp.Models;

public partial class QuitSmartDesktopContext : DbContext
{
    public QuitSmartDesktopContext()
    {
    }

    public QuitSmartDesktopContext(DbContextOptions<QuitSmartDesktopContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Admin> Admins { get; set; }

    public virtual DbSet<AdminLog> AdminLogs { get; set; }

    public virtual DbSet<BadgeDefinition> BadgeDefinitions { get; set; }

    public virtual DbSet<DailyLog> DailyLogs { get; set; }

    public virtual DbSet<HealthInfo> HealthInfos { get; set; }

    public virtual DbSet<HealthTrackingOverview> HealthTrackingOverviews { get; set; }

    public virtual DbSet<MotivationalMessage> MotivationalMessages { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserBadge> UserBadges { get; set; }

    public virtual DbSet<UserBadgeCollection> UserBadgeCollections { get; set; }

    public virtual DbSet<UserOverview> UserOverviews { get; set; }

    public virtual DbSet<UserProfile> UserProfiles { get; set; }

    public virtual DbSet<UserStatistic> UserStatistics { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=localhost;Database=QuitSmartDesktop;User Id=sa;Password=12345;Trusted_Connection=True;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Admin>(entity =>
        {
            entity.HasKey(e => e.AdminId).HasName("PK__Admins__719FE4883712FBA8");

            entity.HasIndex(e => e.Username, "UQ__Admins__536C85E44DFF5B5B").IsUnique();

            entity.HasIndex(e => e.Email, "UQ__Admins__A9D10534EDB24E80").IsUnique();

            entity.Property(e => e.AdminId).HasDefaultValueSql("(newid())");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.Email).HasMaxLength(255);
            entity.Property(e => e.FullName).HasMaxLength(100);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.PasswordHash).HasMaxLength(255);
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.Username).HasMaxLength(50);
        });

        modelBuilder.Entity<AdminLog>(entity =>
        {
            entity.HasKey(e => e.LogId).HasName("PK__AdminLog__5E548648EC792E7F");

            entity.HasIndex(e => e.Action, "IX_AdminLogs_Action");

            entity.HasIndex(e => new { e.AdminId, e.CreatedAt }, "IX_AdminLogs_AdminId_CreatedAt");

            entity.Property(e => e.LogId).HasDefaultValueSql("(newid())");
            entity.Property(e => e.Action).HasMaxLength(100);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.Details).HasMaxLength(500);

            entity.HasOne(d => d.Admin).WithMany(p => p.AdminLogs)
                .HasForeignKey(d => d.AdminId)
                .HasConstraintName("FK__AdminLogs__Admin__0B91BA14");
        });

        modelBuilder.Entity<BadgeDefinition>(entity =>
        {
            entity.HasKey(e => e.BadgeId).HasName("PK__BadgeDef__1918235C406A847F");

            entity.Property(e => e.BadgeId).HasDefaultValueSql("(newid())");
            entity.Property(e => e.BadgeType).HasMaxLength(50);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.IconPath).HasMaxLength(500);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.RequiredValue).HasColumnType("decimal(10, 2)");
        });

        modelBuilder.Entity<DailyLog>(entity =>
        {
            entity.HasKey(e => e.LogId).HasName("PK__DailyLog__5E548648FD7351C3");

            entity.ToTable(tb =>
                {
                    tb.HasTrigger("TR_DailyLogs_CalculateStats");
                    tb.HasTrigger("TR_DailyLogs_UpdatedAt");
                });

            entity.HasIndex(e => e.HasSmoked, "IX_DailyLogs_HasSmoked");

            entity.HasIndex(e => e.LogDate, "IX_DailyLogs_LogDate");

            entity.HasIndex(e => new { e.UserId, e.LogDate }, "IX_DailyLogs_UserId_LogDate");

            entity.HasIndex(e => new { e.UserId, e.LogDate }, "UQ__DailyLog__3368CB929B753C5F").IsUnique();

            entity.Property(e => e.LogId).HasDefaultValueSql("(newid())");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.HasSmoked).HasDefaultValue(false);
            entity.Property(e => e.HealthStatus).HasMaxLength(20);
            entity.Property(e => e.Notes).HasMaxLength(255);
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("(getutcdate())");

            entity.HasOne(d => d.User).WithMany(p => p.DailyLogs)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__DailyLogs__UserI__5EBF139D");
        });

        modelBuilder.Entity<HealthInfo>(entity =>
        {
            entity.HasKey(e => e.InfoId).HasName("PK__HealthIn__4DEC9D7AC2966819");

            entity.ToTable("HealthInfo");

            entity.Property(e => e.InfoId).HasDefaultValueSql("(newid())");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.InfoType).HasMaxLength(50);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.Title).HasMaxLength(200);
        });

        modelBuilder.Entity<HealthTrackingOverview>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("HealthTrackingOverview");

            entity.Property(e => e.FullName).HasMaxLength(100);
            entity.Property(e => e.HealthStatus).HasMaxLength(20);
            entity.Property(e => e.Notes).HasMaxLength(255);
        });

        modelBuilder.Entity<MotivationalMessage>(entity =>
        {
            entity.HasKey(e => e.MessageId).HasName("PK__Motivati__C87C0C9C11BFB62F");

            entity.Property(e => e.MessageId).HasDefaultValueSql("(newid())");
            entity.Property(e => e.Content).HasMaxLength(1000);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.MessageType).HasMaxLength(50);
            entity.Property(e => e.Title).HasMaxLength(200);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__Users__1788CC4C68E3DB42");

            entity.ToTable(tb => tb.HasTrigger("TR_Users_UpdatedAt"));

            entity.HasIndex(e => e.Email, "IX_Users_Email");

            entity.HasIndex(e => e.IsActive, "IX_Users_IsActive");

            entity.HasIndex(e => e.Username, "IX_Users_Username");

            entity.HasIndex(e => e.Username, "UQ__Users__536C85E41C60177A").IsUnique();

            entity.HasIndex(e => e.Email, "UQ__Users__A9D10534317D7607").IsUnique();

            entity.Property(e => e.UserId).HasDefaultValueSql("(newid())");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.Email).HasMaxLength(255);
            entity.Property(e => e.FullName).HasMaxLength(100);
            entity.Property(e => e.Gender).HasMaxLength(10);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.PasswordHash).HasMaxLength(255);
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.Username).HasMaxLength(50);
        });

        modelBuilder.Entity<UserBadge>(entity =>
        {
            entity.HasKey(e => e.UserBadgeId).HasName("PK__UserBadg__11D3AC9F678F1399");

            entity.HasIndex(e => e.EarnedAt, "IX_UserBadges_EarnedAt");

            entity.HasIndex(e => e.UserId, "IX_UserBadges_UserId");

            entity.HasIndex(e => new { e.UserId, e.BadgeId }, "UQ__UserBadg__C6194E78CB0DA6BB").IsUnique();

            entity.Property(e => e.UserBadgeId).HasDefaultValueSql("(newid())");
            entity.Property(e => e.EarnedAt).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.IsNotified).HasDefaultValue(false);

            entity.HasOne(d => d.Badge).WithMany(p => p.UserBadges)
                .HasForeignKey(d => d.BadgeId)
                .HasConstraintName("FK__UserBadge__Badge__75A278F5");

            entity.HasOne(d => d.User).WithMany(p => p.UserBadges)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__UserBadge__UserI__74AE54BC");
        });

        modelBuilder.Entity<UserBadgeCollection>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("UserBadgeCollection");

            entity.Property(e => e.BadgeDescription).HasMaxLength(500);
            entity.Property(e => e.BadgeName).HasMaxLength(100);
            entity.Property(e => e.BadgeType).HasMaxLength(50);
            entity.Property(e => e.FullName).HasMaxLength(100);
            entity.Property(e => e.IconPath).HasMaxLength(500);
        });

        modelBuilder.Entity<UserOverview>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("UserOverview");

            entity.Property(e => e.Email).HasMaxLength(255);
            entity.Property(e => e.FullName).HasMaxLength(100);
            entity.Property(e => e.Gender).HasMaxLength(10);
            entity.Property(e => e.PricePerPack).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.QuitReason).HasMaxLength(500);
            entity.Property(e => e.TotalMoneySaved).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.Username).HasMaxLength(50);
        });

        modelBuilder.Entity<UserProfile>(entity =>
        {
            entity.HasKey(e => e.ProfileId).HasName("PK__UserProf__290C88E4083EF73C");

            entity.ToTable(tb => tb.HasTrigger("TR_UserProfiles_UpdatedAt"));

            entity.Property(e => e.ProfileId).HasDefaultValueSql("(newid())");
            entity.Property(e => e.CigarettesPerPack).HasDefaultValue(20);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.PricePerPack).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.QuitReason).HasMaxLength(500);
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("(getutcdate())");

            entity.HasOne(d => d.User).WithMany(p => p.UserProfiles)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__UserProfi__UserI__5629CD9C");
        });

        modelBuilder.Entity<UserStatistic>(entity =>
        {
            entity.HasKey(e => e.StatId).HasName("PK__UserStat__3A162D3ED80F0077");

            entity.HasIndex(e => e.UserId, "UQ__UserStat__1788CC4DB4C08EA5").IsUnique();

            entity.Property(e => e.StatId).HasDefaultValueSql("(newid())");
            entity.Property(e => e.CurrentStreak).HasDefaultValue(0);
            entity.Property(e => e.LastCalculatedAt).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.LongestStreak).HasDefaultValue(0);
            entity.Property(e => e.TotalDaysQuit).HasDefaultValue(0);
            entity.Property(e => e.TotalMoneySaved)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(10, 2)");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("(getutcdate())");

            entity.HasOne(d => d.User).WithOne(p => p.UserStatistic)
                .HasForeignKey<UserStatistic>(d => d.UserId)
                .HasConstraintName("FK__UserStati__UserI__693CA210");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
