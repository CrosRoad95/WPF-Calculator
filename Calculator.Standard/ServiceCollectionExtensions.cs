using Microsoft.Extensions.DependencyInjection;
using Calculator.Core;

namespace Calculator.Standard;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddStandardCalculator(this IServiceCollection services)
    {
        services.AddCalculator<Calculator>("Standardowy", "Kalkulator", "🖩", true);
        //services.AddCalculator<Calculator>("Test123", "Kalkulator", "X");
        //services.AddCalculator<Calculator>("TestABC", "Kalkulator", "Y");
        return services;
    }
}
