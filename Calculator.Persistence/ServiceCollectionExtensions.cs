using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Calculator.Persistence;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddPeristence(this IServiceCollection services)
    {
        services.AddDbContextFactory<CalculatorDBContext>(options =>
            options.UseSqlite("Data Source=Calculator.db"));
        services.AddSingleton<SavedEntriesRepository>();
        return services;
    }
}
