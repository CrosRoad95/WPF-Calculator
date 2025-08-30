using Microsoft.EntityFrameworkCore;

namespace Calculator.Persistence;

public sealed class SavedEntriesRepository
{
    private readonly IDbContextFactory<CalculatorDBContext> _dbContextFactory;

    public SavedEntriesRepository(IDbContextFactory<CalculatorDBContext> dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
    }

    public async Task Add(string calculatorType, string input, string output)
    {
        await using var db = _dbContextFactory.CreateDbContext();

        await db.Database.ExecuteSqlInterpolatedAsync($@"
            INSERT INTO SavedEntries (CalculatorType, Input, Output, DateTime)
            VALUES ({calculatorType}, {input}, {output}, {DateTime.Now})
        ");
    }

    public async Task<Tuple<string, string>[]> GetAll(string calculatorType)
    {
        await using var db = _dbContextFactory.CreateDbContext();
        var query = db.SavedEntries
            .Where(x => x.CalculatorType == calculatorType)
            .OrderByDescending(x => x.DateTime)
            .Select(x => new Tuple<string, string>(x.Input, x.Output));

        return await query.ToArrayAsync();
    }

    public async Task Clear(string calculatorType)
    {
        await using var db = _dbContextFactory.CreateDbContext();
        var query = db.SavedEntries
            .Where(x => x.CalculatorType == calculatorType);

        await query.ExecuteDeleteAsync();
    }
}
