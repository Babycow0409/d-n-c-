using Microsoft.EntityFrameworkCore;
using MyWebApi.Models;

namespace MyWebApi.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; } = null!;
        public DbSet<Poi> Pois { get; set; } = null!;
        public DbSet<PoiContent> PoiContents { get; set; } = null!;
        public DbSet<Audio> Audios { get; set; } = null!;
        public DbSet<ListenLog> ListenLogs { get; set; } = null!;

        // Khôi phục lại các bảng cũ của bạn
        public DbSet<Language> Languages { get; set; } = null!;
        public DbSet<Explanation> Explanations { get; set; } = null!;
        public DbSet<ExplanationContent> ExplanationContents { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Audio>()
                .HasOne(a => a.PoiContent)
                .WithOne(p => p.Audio)
                .HasForeignKey<Audio>(a => a.PoiContentId);

            modelBuilder.Entity<PoiContent>()
                .HasIndex(p => new { p.PoiId, p.LanguageCode })
                .IsUnique();

            modelBuilder.Entity<ListenLog>()
                .HasOne(l => l.User)
                .WithMany(u => u.ListenLogs)
                .HasForeignKey(l => l.UserId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
