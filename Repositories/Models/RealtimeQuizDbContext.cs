using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Repositories.Models;

public partial class RealtimeQuizDbContext : DbContext
{
    public RealtimeQuizDbContext()
    {
    }

    public RealtimeQuizDbContext(DbContextOptions<RealtimeQuizDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Administrator> Administrators { get; set; }

    public virtual DbSet<Answer> Answers { get; set; }

    public virtual DbSet<Question> Questions { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Room> Rooms { get; set; }

    public virtual DbSet<RoomQuestion> RoomQuestions { get; set; }

    public virtual DbSet<Score> Scores { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserAnswer> UserAnswers { get; set; }

    public virtual DbSet<UserRoom> UserRooms { get; set; }


    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    { optionsBuilder.UseSqlServer(GetConnectionString()); }

    private string GetConnectionString()
    {
        IConfiguration configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", true, true).Build();
        return configuration["ConnectionStrings:DefaultConnection"];
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Administrator>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Administ__3214EC078BC13D4C");

            entity.ToTable("Administrator");

            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("isActive");
            entity.Property(e => e.Password).HasMaxLength(256);
            entity.Property(e => e.RoleId).HasDefaultValue(1);
            entity.Property(e => e.UserName).HasMaxLength(50);

            entity.HasOne(d => d.Role).WithMany(p => p.Administrators)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Administrator_Role");
        });

        modelBuilder.Entity<Answer>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Answer__3214EC07182FE946");

            entity.ToTable("Answer");

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.Content).HasMaxLength(255);

            entity.HasOne(d => d.Question).WithMany(p => p.Answers)
                .HasForeignKey(d => d.QuestionId)
                .HasConstraintName("FK_Answer_Question");
        });

        modelBuilder.Entity<Question>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Question__3214EC079027C509");

            entity.ToTable("Question");

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.Content).HasMaxLength(500);
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Role__3214EC072002E672");

            entity.ToTable("Role");

            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("isActive");
            entity.Property(e => e.RoleName).HasMaxLength(50);
        });

        modelBuilder.Entity<Room>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Room__3214EC079F144705");

            entity.ToTable("Room");

            entity.HasIndex(e => e.RoomCode, "UQ__Room__4F9D5231851EE2C9").IsUnique();

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("isActive");
            entity.Property(e => e.Name).HasMaxLength(255);
            entity.Property(e => e.RoomCode).ValueGeneratedOnAdd();
        });

        modelBuilder.Entity<RoomQuestion>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__RoomQues__3214EC07A37592C4");

            entity.ToTable("RoomQuestion");

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");

            entity.HasOne(d => d.Question).WithMany(p => p.RoomQuestions)
                .HasForeignKey(d => d.QuestionId)
                .HasConstraintName("FK_RoomQuestion_Question");

            entity.HasOne(d => d.Room).WithMany(p => p.RoomQuestions)
                .HasForeignKey(d => d.RoomId)
                .HasConstraintName("FK_RoomQuestion_Room");
        });

        modelBuilder.Entity<Score>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Score__3214EC07545EB8C0");

            entity.ToTable("Score");

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.TotalPoints).HasDefaultValue(0);

            entity.HasOne(d => d.Room).WithMany(p => p.Scores)
                .HasForeignKey(d => d.RoomId)
                .HasConstraintName("FK_Score_Room");

            entity.HasOne(d => d.User).WithMany(p => p.Scores)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_Score_User");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Users__3214EC0751630E01");

            entity.HasIndex(e => e.Username, "UQ__Users__536C85E4B7A2C032").IsUnique();

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Username).HasMaxLength(100);
        });

        modelBuilder.Entity<UserAnswer>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__UserAnsw__3214EC0778AAC50C");

            entity.ToTable("UserAnswer");

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.AnsweredAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.IsTimeOut).HasDefaultValue(false);

            entity.HasOne(d => d.Answer).WithMany(p => p.UserAnswers)
                .HasForeignKey(d => d.AnswerId)
                .HasConstraintName("FK_UserAnswer_Answer");

            entity.HasOne(d => d.Question).WithMany(p => p.UserAnswers)
                .HasForeignKey(d => d.QuestionId)
                .HasConstraintName("FK_UserAnswer_Question");

            entity.HasOne(d => d.Room).WithMany(p => p.UserAnswers)
                .HasForeignKey(d => d.RoomId)
                .HasConstraintName("FK_UserAnswer_Room");

            entity.HasOne(d => d.User).WithMany(p => p.UserAnswers)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_UserAnswer_User");
        });

        modelBuilder.Entity<UserRoom>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__UserRoom__3214EC0742D07B42");

            entity.ToTable("UserRoom");

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.JoinedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.Room).WithMany(p => p.UserRooms)
                .HasForeignKey(d => d.RoomId)
                .HasConstraintName("FK_UserRoom_Room");

            entity.HasOne(d => d.User).WithMany(p => p.UserRooms)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_UserRoom_User");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
