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

        public void Configure(EntityTypeBuilder<ModCase> builder) {
            builder.Property(u => u.CreatedAt).IsRequired(true).HasDefaultValueSql("now()");
        }

        public void Configure(EntityTypeBuilder<ModCaseComments> builder)
        {
            builder.HasKey(u => u.ModCaseId);
            builder.Property(u => u.CreatedAt).IsRequired(true).HasDefaultValueSql("now()");

            // FK - configuraiton
            builder.HasOne(u => u.ModCase).WithMany().HasForeignKey(u => u.ModCaseId).IsRequired(true);
        }
    }
}