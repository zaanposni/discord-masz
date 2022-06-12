using MASZ.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace MASZ.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) { }

        public DbSet<ModCase> ModCases { get; set; }
        public DbSet<GuildConfig> GuildConfigs { get; set; }
        public DbSet<ModCaseComment> ModCaseComments { get; set; }
        public DbSet<AutoModerationEvent> AutoModerationEvents { get; set; }
        public DbSet<AutoModerationConfig> AutoModerationConfigs { get; set; }
        public DbSet<CaseTemplate> CaseTemplates { get; set; }
        public DbSet<GuildMotd> GuildMotds { get; set; }
        public DbSet<APIToken> APITokens { get; set; }
        public DbSet<UserInvite> UserInvites { get; set; }
        public DbSet<UserMapping> UserMappings { get; set; }
        public DbSet<UserNote> UserNotes { get; set; }
        public DbSet<GuildLevelAuditLogConfig> GuildLevelAuditLogConfigs { get; set; }
        public DbSet<AppSettings> AppSettings { get; set; }
        public DbSet<ScheduledMessage> ScheduledMessages { get; set; }
        public DbSet<Appeal> Appeals { get; set; }
        public DbSet<AppealStructure> AppealStructures { get; set; }
        public DbSet<AppealAnswer> AppealAnswers { get; set; }
        public DbSet<ModCaseMapping> ModCaseMappings { get; set; }
        public DbSet<ZalgoConfig> ZalgoConfigs { get; set; }

        public void Configure(EntityTypeBuilder<ModCase> builder)
        {
            builder.Property(u => u.CreatedAt).IsRequired(true).HasDefaultValueSql("now()");
            builder.Property(u => u.Valid).HasDefaultValue(true);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (string.Equals("true", Environment.GetEnvironmentVariable("ENABLE_SQL_LOGGING")))
            {
                optionsBuilder.UseLoggerFactory(LoggerFactory.Create(builder =>
                {
                    builder.AddConsole();
                })).EnableSensitiveDataLogging();
            }
        }

        protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
        {
            configurationBuilder.Properties<ulong[]>().HaveConversion<ULAConverter>();
            configurationBuilder.Properties<string[]>().HaveConversion<SAConverter>();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            SAComparer stringArrayComparer = new();

            ULAComparer ulongArrayComparer = new();

            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
                foreach (var property in entityType.GetProperties())
                {
                    if (property.ClrType == typeof(ulong[]))
                        property.SetValueComparer(ulongArrayComparer);
                    else if (property.ClrType == typeof(string[]))
                        property.SetValueComparer(stringArrayComparer);
                }

            modelBuilder.Entity<ModCase>()
                .HasKey(o => new { o.Id });

            modelBuilder.Entity<ModCase>()
                .Property(p => p.CaseId)
                .IsRequired(true);

            modelBuilder.Entity<ModCase>()
                .Property(p => p.GuildId)
                .IsRequired(true);

            modelBuilder.Entity<ModCaseComment>()
                .HasOne(c => c.ModCase)
                .WithMany(c => c.Comments)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ModCaseMapping>()
                .HasOne(c => c.CaseA)
                .WithMany(c => c.MappingsA)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ModCaseMapping>()
                .HasOne(c => c.CaseB)
                .WithMany(c => c.MappingsB)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<AppealAnswer>()
                .HasOne(c => c.Appeal)
                .WithMany()
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<AppealAnswer>()
                .HasOne(c => c.AppealQuestion)
                .WithMany()
                .OnDelete(DeleteBehavior.Cascade);
        }
    }

    public class ULAConverter : ValueConverter<ulong[], string>
    {
        public ULAConverter() :
            base(
                v => string.Join(',', v),
                v => v.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(x => ulong.Parse(x)).ToArray(),
                null
            )
        { }
    }

    public class ULAComparer : ValueComparer<ulong[]>
    {
        public ULAComparer()
            : base(
                (c1, c2) => c1.SequenceEqual(c2),
                c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                c => (ulong[])c.Clone()
            )
        { }
    }

    public class SAConverter : ValueConverter<string[], string>
    {
        public SAConverter() :
            base(
                v => string.Join(',', v),
                v => v.Split(',', StringSplitOptions.RemoveEmptyEntries),
                null
            )
        { }
    }

    public class SAComparer : ValueComparer<string[]>
    {
        public SAComparer()
            : base(
                (c1, c2) => c1.SequenceEqual(c2),
                c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                c => (string[])c.Clone()
            )
        { }
    }
}