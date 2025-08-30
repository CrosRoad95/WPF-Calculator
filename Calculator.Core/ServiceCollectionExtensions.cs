using Microsoft.Extensions.DependencyInjection;

namespace Calculator.Core;

public delegate void SavedCallback(string input, string output);

public interface ICalculatorWindow
{
    public event SavedCallback? Saved;
    public void Restore(string input, string output);
}

public sealed record CalculatorInstance(Type UserControlType, string Name, string Category, string Icon, bool IsDefault);

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCalculator<T>(this IServiceCollection services, string name, string category, string icon, bool isDefault = false)
        where T : ICalculatorWindow
    {
        services.AddSingleton(new CalculatorInstance(typeof(T), name, category, icon, isDefault));
        return services;
    } 
}
