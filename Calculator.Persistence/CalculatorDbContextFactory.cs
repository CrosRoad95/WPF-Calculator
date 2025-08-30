using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Calculator.Persistence;

public class CalculatorDbContextFactory : IDesignTimeDbContextFactory<CalculatorDBContext>
{
    public CalculatorDBContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<CalculatorDBContext>();
        optionsBuilder.UseSqlite("Data Source=Calculator.db");

        return new CalculatorDBContext(optionsBuilder.Options);
    }
}
