using masz.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace masz.data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) { }

        public DbSet<ModCase> ModCases { get; set; }        
        public DbSet<ModCaseComments> ModCaseComments { get; set; }
        public DbSet<GuildConfig> GuildConfigs { get; set; }

        public void Configure(EntityTypeBuilder<ModCase> builder) {
            builder.Property(u => u.CreatedAt).IsRequired(true).HasDefaultValueSql("now()");
            builder.Property(u => u.Severity).HasDefaultValue(0);
            builder.Property(u => u.Valid).HasDefaultValue(true);
        }

        public void Configure(EntityTypeBuilder<ModCaseComments> builder)
        {
            builder.HasKey(u => u.ModCaseId);
            builder.Property(u => u.CreatedAt).IsRequired(true).HasDefaultValueSql("now()");

            // FK - configuraiton
            builder.HasOne(u => u.ModCase).WithMany().HasForeignKey(u => u.ModCaseId).IsRequired(true);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ModCase>()
                .HasKey(o => new { o.Id });
            
            modelBuilder.Entity<ModCaseComments>()
                .HasKey(o => new { o.Id });
            modelBuilder.Entity<ModCaseComments>()
                .HasOne(p => p.ModCase)
                .WithMany(b => b.ModCaseComments)
                .HasForeignKey(o => new { o.ModCaseId });
        }
    }
}