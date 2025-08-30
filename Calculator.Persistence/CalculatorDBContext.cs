using Microsoft.EntityFrameworkCore;

namespace Calculator.Persistence;

public sealed record SavedEntry(string CalculatorType, string Input, string Output, DateTime DateTime);

public class CalculatorDBContext : DbContext
{
    public CalculatorDBContext(DbContextOptions options) : base(options) { }
    public DbSet<SavedEntry> SavedEntries { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<SavedEntry>()
            .HasNoKey()
            .ToTable(nameof(SavedEntries));

        base.OnModelCreating(modelBuilder);
    }
}
