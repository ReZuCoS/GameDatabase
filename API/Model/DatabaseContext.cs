using Microsoft.EntityFrameworkCore;
using Shared.Model;

namespace API.Model
{
    public class DatabaseContext : DbContext
    {
        public virtual DbSet<Language> Languages { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<GameStatus> GameStatuses { get; set; }
        public virtual DbSet<Game> Games { get; set; }
        public virtual DbSet<GameTranslation> GameTranslations { get; set; }
        public virtual DbSet<Genre> Genres { get; set; }
        public virtual DbSet<Tag> Tags { get; set; }
        public virtual DbSet<UserGame> UserGames { get; set; }

        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Language>(e =>
            {
                e.ToTable("languages");
                e.HasKey(e => e.Key);

                e.Property(e => e.Key).HasColumnName("key");
                e.Property(e => e.Name).HasColumnName("name");

                e.HasMany(e => e.Users)
                    .WithOne(e => e.Language)
                    .HasForeignKey(e => e.LanguageKey);
                e.HasMany(e => e.GameStatuses)
                    .WithOne(e => e.Language)
                    .HasForeignKey(e => e.LanguageKey);
                e.HasMany(e => e.GameTranslations)
                    .WithOne(e => e.Language)
                    .HasForeignKey(e => e.LanguageKey);
            });

            modelBuilder.Entity<User>(e =>
            {
                e.ToTable("users");
                e.HasKey(e => e.Login);

                e.Property(e => e.Login).HasColumnName("login");
                e.Property(e => e.Password).HasColumnName("password");
                e.Property(e => e.Salt).HasColumnName("salt");
                e.Property(e => e.TotpKey).HasColumnName("totp_key");
                e.Property(e => e.TotpRecoveries).HasColumnName("totp_recoveries");
                e.Property(e => e.ProfileImage).HasColumnName("profile_image");
                e.Property(e => e.LanguageKey).HasColumnName("language_key");

                e.HasMany(e => e.UserGames)
                    .WithOne(e => e.User)
                    .HasForeignKey(e => e.UserLogin);
            });

            modelBuilder.Entity<GameStatus>(e =>
            {
                e.ToTable("game_statuses");
                e.HasKey(e => e.ID);

                e.Property(e => e.ID).HasColumnName("id");
                e.Property(e => e.Name).HasColumnName("name");
                e.Property(e => e.LanguageKey).HasColumnName("language_key");

                e.HasMany(e => e.UserGames)
                    .WithOne(e => e.GameStatus)
                    .HasForeignKey(e => e.Status);
            });

            modelBuilder.Entity<Game>(e =>
            {
                e.ToTable("games");
                e.HasKey(e => e.ID);

                e.Property(e => e.ID).HasColumnName("id");
                e.Property(e => e.ImageVertical).HasColumnName("image_vertical");
                e.Property(e => e.ImageHorizontal).HasColumnName("image_horizontal");
                e.Property(e => e.AvgPlaytimeInHours).HasColumnName("avg_playtime_in_hours");
                e.Property(e => e.ReleaseDate).HasColumnName("release_date");

                e.HasMany(e => e.UserGames)
                    .WithOne(e => e.Game)
                    .HasForeignKey(e => e.GameID);
                e.HasMany(e => e.GameTranslations)
                    .WithOne(e => e.Game)
                    .HasForeignKey(e => e.GameID);
                e.HasMany(e => e.Genres)
                    .WithMany(e => e.Games);
                e.HasMany(e => e.Tags)
                    .WithMany(e => e.Games);
            });

            modelBuilder.Entity<GameTranslation>(e =>
            {
                e.ToTable("game_translations");
                e.HasKey(e => e.GameID);

                e.Property(e => e.GameID).HasColumnName("game_id");
                e.Property(e => e.Title).HasColumnName("title");
                e.Property(e => e.Description).HasColumnName("description");
                e.Property(e => e.LanguageKey).HasColumnName("language_key");
            });

            modelBuilder.Entity<Genre>(e =>
            {
                e.ToTable("genres");
                e.HasKey(e => e.ID);

                e.Property(e => e.ID).HasColumnName("id");
                e.Property(e => e.Name).HasColumnName("name");
            });

            modelBuilder.Entity<Tag>(e =>
            {
                e.ToTable("tags");
                e.HasKey(e => e.ID);

                e.Property(e => e.ID).HasColumnName("id");
                e.Property(e => e.Name).HasColumnName("name");
            });

            modelBuilder.Entity<UserGame>(e =>
            {
                e.ToTable("user_games");
                e.HasKey(e => new { e.GameID, e.UserLogin });

                e.Property(e => e.GameID).HasColumnName("game_id");
                e.Property(e => e.UserLogin).HasColumnName("user_login");
                e.Property(e => e.Status).HasColumnName("status");
                e.Property(e => e.CustomImageVertical).HasColumnName("custom_image_vertical");
                e.Property(e => e.CustomImageHorizontal).HasColumnName("custom_image_horizontal");
                e.Property(e => e.UserRate).HasColumnName("user_rate");
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}
