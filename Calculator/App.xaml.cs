using Calculator.Core;
using Calculator.Persistence;
using Calculator.Standard;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Configuration;
using System.Data;
using System.Windows;

namespace Kalkulator;

public sealed class Calculators
{
    private readonly Dictionary<string, CalculatorInstance[]> _calculatorsByType;

    public IReadOnlyDictionary<string, CalculatorInstance[]> CalculatorsByType => _calculatorsByType;
    public CalculatorInstance Default => _calculatorsByType.SelectMany(x => x.Value).Single(x => x.IsDefault);
    public Calculators(IServiceProvider serviceProvider)
    {
        _calculatorsByType = serviceProvider.GetRequiredService<IEnumerable<CalculatorInstance>>()
            .GroupBy(x => x.Category)
            .ToDictionary(x => x.Key, y => y.ToArray());
    }
}

public partial class App : Application
{
    public static IServiceProvider ServiceProvider { get; private set; } = default!;

    protected override void OnStartup(StartupEventArgs e)
    {
        var services = new ServiceCollection();

        services.AddSingleton<MainWindow>();
        services.AddSingleton<Calculators>();
        services.AddPeristence();

        services.AddStandardCalculator();

        ServiceProvider = services.BuildServiceProvider();

        Task.Run(async () =>
        {
            await ServiceProvider.GetRequiredService<CalculatorDBContext>().Database.MigrateAsync();
        }).Wait();

        var mainWindow = ServiceProvider.GetRequiredService<MainWindow>();
        mainWindow.Show();
    }
}
