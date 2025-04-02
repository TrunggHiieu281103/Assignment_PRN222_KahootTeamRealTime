using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

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

    public virtual DbSet<Answer> Answers { get; set; }

    public virtual DbSet<Question> Questions { get; set; }

    public virtual DbSet<Room> Rooms { get; set; }

    public virtual DbSet<RoomQuestion> RoomQuestions { get; set; }

    public virtual DbSet<Score> Scores { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserAnswer> UserAnswers { get; set; }

    public virtual DbSet<UserRoom> UserRooms { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        //=> optionsBuilder.UseSqlServer("Server=(local);Database=RealtimeQuizDB;Uid=sa;Pwd=12345;TrustServerCertificate=True");
    => optionsBuilder.UseSqlServer("Server=LAPTOP-39B7IASC\\SQLEXPRESS;Database=RealtimeQuizDB;Uid=sa;Pwd=1;TrustServerCertificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Answer>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Answer__3214EC075BC6E812");

            entity.ToTable("Answer");

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.Content).HasMaxLength(255);

            entity.HasOne(d => d.Question).WithMany(p => p.Answers)
                .HasForeignKey(d => d.QuestionId)
                .HasConstraintName("FK_Answer_Question");
        });

        modelBuilder.Entity<Question>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Question__3214EC07FC6084B0");

            entity.ToTable("Question");

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.Content).HasMaxLength(500);
        });

        modelBuilder.Entity<Room>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Room__3214EC076F6517E0");

            entity.ToTable("Room");

            entity.HasIndex(e => e.RoomCode, "UQ__Room__4F9D5231B04E46C1").IsUnique();

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Name).HasMaxLength(255);
            entity.Property(e => e.RoomCode).ValueGeneratedOnAdd();
        });

        modelBuilder.Entity<RoomQuestion>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__RoomQues__3214EC078130BA38");

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
            entity.HasKey(e => e.Id).HasName("PK__Score__3214EC07A0273D6B");

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
            entity.HasKey(e => e.Id).HasName("PK__Users__3214EC07FB0EB22D");

            entity.HasIndex(e => e.Username, "UQ__Users__536C85E4007F5144").IsUnique();

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Username).HasMaxLength(100);
        });

        modelBuilder.Entity<UserAnswer>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__UserAnsw__3214EC073D943E2C");

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
            entity.HasKey(e => e.Id).HasName("PK__UserRoom__3214EC0707FE1804");

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
