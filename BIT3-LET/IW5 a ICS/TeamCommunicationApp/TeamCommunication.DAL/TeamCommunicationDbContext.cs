using Microsoft.EntityFrameworkCore;
using TeamCommunication.DAL.Entities;

namespace TeamCommunication.DAL
{
    public class TeamCommunicationDbContext : DbContext
    {
        public TeamCommunicationDbContext()
        {
        }

        public TeamCommunicationDbContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<User>()
                .HasMany(t => t.Topics)
                .WithOne(t => t.User)
                .IsRequired(false);
            modelBuilder.Entity<User>()
                .HasMany(t => t.Comments)
                .WithOne(t => t.User)
                .IsRequired(false);
            modelBuilder.Entity<User>()
                .HasMany(t => t.UserTeams)
                .WithOne(t => t.User);

            modelBuilder.Entity<Team>()
                .HasMany(t => t.UserTeams)
                .WithOne(t => t.Team);
            modelBuilder.Entity<Team>()
                .HasMany(t => t.Topics)
                .WithOne(t => t.Team)
                .IsRequired(false);

            modelBuilder.Entity<Topic>()
                .HasMany(t => t.Comments)
                .WithOne(t => t.Topic)
                .IsRequired(false);

            modelBuilder.Entity<UserTeam>()
                .HasKey(bc => new { bc.idTeam, bc.idUser });
            modelBuilder.Entity<UserTeam>()
                .HasOne(t => t.User)
                .WithMany(t => t.UserTeams)
                .HasForeignKey(bc => bc.idUser);
            modelBuilder.Entity<UserTeam>()
                .HasOne(t => t.Team)
                .WithMany(t => t.UserTeams)
                .HasForeignKey(bc => bc.idTeam);
        }


        public DbSet<User> Users { get; set; }
        public DbSet<Team> Teams { get; set; }
        public DbSet<Topic> Topics { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<UserTeam> UserTeams { get; set; }

    }
}
