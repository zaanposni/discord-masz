using System;
using masz.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;

namespace masz.data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) { }

        //static LoggerFactory object
        public static readonly ILoggerFactory loggerFactory = LoggerFactory.Create(builder => {
            builder.AddConsole();
        });

        public DbSet<ModCase> ModCases { get; set; }
        public DbSet<GuildConfig> GuildConfigs { get; set; }
        public DbSet<ModCaseComment> ModCaseComments { get; set; }
        public DbSet<AutoModerationEvent> AutoModerationEvents { get; set; }
        public DbSet<AutoModerationConfig> AutoModerationConfigs { get; set; }
        public DbSet<CaseTemplate> CaseTemplates { get; set; }
        public DbSet<GuildMotd> GuildMotds { get; set; }

        public void Configure(EntityTypeBuilder<ModCase> builder) {
            builder.Property(u => u.CreatedAt).IsRequired(true).HasDefaultValueSql("now()");
            builder.Property(u => u.Valid).HasDefaultValue(true);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (String.Equals("true", System.Environment.GetEnvironmentVariable("ENABLE_SQL_LOGGING"))) {
                optionsBuilder.UseLoggerFactory(loggerFactory).EnableSensitiveDataLogging();
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ModCase>()
                .HasKey(o => new { o.Id });

            modelBuilder.Entity<ModCase>()
                .Property(p => p.CaseId)
                .IsRequired(true);

            modelBuilder.Entity<ModCase>()
                .Property(p => p.GuildId)
                .IsRequired(true);

            modelBuilder.Entity<ModCase>()
                .Property(e => e.Labels)
                .HasConversion(
                    v => string.Join(',', v),
                    v => v.Split(',', StringSplitOptions.RemoveEmptyEntries));

            modelBuilder.Entity<CaseTemplate>()
                .Property(e => e.CaseLabels)
                .HasConversion(
                    v => string.Join(',', v),
                    v => v.Split(',', StringSplitOptions.RemoveEmptyEntries));
            
            modelBuilder.Entity<AutoModerationConfig>()
                .Property(e => e.IgnoreChannels)
                .HasConversion(
                    v => string.Join(',', v),
                    v => v.Split(',', StringSplitOptions.RemoveEmptyEntries));

            modelBuilder.Entity<AutoModerationConfig>()
                .Property(e => e.IgnoreRoles)
                .HasConversion(
                    v => string.Join(',', v),
                    v => v.Split(',', StringSplitOptions.RemoveEmptyEntries));
            
            modelBuilder.Entity<ModCaseComment>()
                .HasOne(c => c.ModCase)
                .WithMany(c => c.Comments)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}