using Google.Protobuf.WellKnownTypes;
using Microsoft.EntityFrameworkCore;

namespace GrpcServer.Models
{
    public class Context : DbContext
    {
        public DbSet<PondData> PondData { get; set; } = null!;

        public Context(DbContextOptions options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PondData>()
                .HasKey(e => e.EntryId);

            modelBuilder
                .Entity<PondData>()
                .Property(e => e.EntryId)
                .ValueGeneratedOnAdd();

            modelBuilder
                .Entity<PondData>()
                .Property(e => e.CreatedAt)
                .HasConversion(
                      v => v.ToDateTime(),
                      v => v.ToTimestamp()
                      );
        }
    }
}
