using System;
using System.Collections.Generic;
using Dartsmanager.Models;
using Dartsmanager.Data;
using Microsoft.EntityFrameworkCore;

namespace Dartsmanager.Data;

public partial class DbDartsmanagerContext : DbContext
{
    public DbDartsmanagerContext()
    {
    }

    public DbDartsmanagerContext(DbContextOptions<DbDartsmanagerContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Adress> Adresses { get; set; }

    public virtual DbSet<Country> Countries { get; set; }

    public virtual DbSet<Game> Games { get; set; }

    public virtual DbSet<GameScore> GameScores { get; set; }

    public virtual DbSet<Group> Groups { get; set; }

    public virtual DbSet<GroupPlayer> GroupPlayers { get; set; }

    public virtual DbSet<Player> Players { get; set; }

    public virtual DbSet<Registration> Registrations { get; set; }

    public virtual DbSet<Status> Statuses { get; set; }

    public virtual DbSet<Tournament> Tournaments { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
//#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlite("Filename=DB_Dartsmanager.db");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Adress>(entity =>
        {
            entity.HasOne(d => d.Country).WithMany(p => p.Adresses).HasForeignKey(d => d.CountryId);
        });

        modelBuilder.Entity<Game>(entity =>
        {
            entity.HasOne(d => d.Group).WithMany(p => p.Games)
                .HasForeignKey(d => d.GroupId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(d => d.Player1).WithMany(p => p.GamePlayer1s).HasForeignKey(d => d.Player1Id);

            entity.HasOne(d => d.Player2).WithMany(p => p.GamePlayer2s).HasForeignKey(d => d.Player2Id);

            entity.HasOne(d => d.Tournament).WithMany(p => p.Games)
                .HasForeignKey(d => d.TournamentId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<GameScore>(entity =>
        {
            entity.HasOne(d => d.Game).WithMany(p => p.GameScores).HasForeignKey(d => d.GameId);

            entity.HasOne(d => d.Player).WithMany(p => p.GameScores).HasForeignKey(d => d.PlayerId);
        });

        modelBuilder.Entity<Group>(entity =>
        {
            entity.HasIndex(e => new { e.TournamentId, e.GroepNummer }, "IX_Groups_TournamentId_GroepNummer").IsUnique();

            entity.HasOne(d => d.Tournament).WithMany(p => p.Groups).HasForeignKey(d => d.TournamentId);
        });

        modelBuilder.Entity<GroupPlayer>(entity =>
        {
            entity.HasOne(d => d.Group).WithMany(p => p.GroupPlayers).HasForeignKey(d => d.GroupId);

            entity.HasOne(d => d.Player).WithMany(p => p.GroupPlayers).HasForeignKey(d => d.PlayerId);
        });

        modelBuilder.Entity<Player>(entity =>
        {
            entity.Property(e => e.IsDummy).HasDefaultValue(0);

            entity.HasOne(d => d.Adres).WithMany(p => p.Players).HasForeignKey(d => d.AdresId);
        });

        modelBuilder.Entity<Registration>(entity =>
        {
            entity.HasIndex(e => new { e.TournamentId, e.PlayerId }, "IX_Registrations_TournamentId_PlayerId").IsUnique();

            entity.HasOne(d => d.Player).WithMany(p => p.Registrations).HasForeignKey(d => d.PlayerId);

            entity.HasOne(d => d.Tournament).WithMany(p => p.Registrations).HasForeignKey(d => d.TournamentId);
        });

        modelBuilder.Entity<Tournament>(entity =>
        {
            entity.HasOne(d => d.Adres).WithMany(p => p.Tournaments).HasForeignKey(d => d.AdresId);

            entity.HasOne(d => d.Status).WithMany(p => p.Tournaments).HasForeignKey(d => d.StatusId);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasIndex(e => e.PlayerId, "IX_Users_PlayerId").IsUnique();

            entity.Property(e => e.PlayerIdBevestigd).HasColumnType("BOOLEAN");

            entity.HasOne(d => d.Player).WithOne(p => p.User).HasForeignKey<User>(d => d.PlayerId);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
