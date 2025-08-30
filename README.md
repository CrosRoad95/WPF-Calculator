# Calculator App using WfF
![Alt text](example.gif)

# Solution projects hierarchy:
<img width="412" height="251" alt="image" src="https://github.com/user-attachments/assets/e124c2fa-1cda-4d54-b8c3-d66ac990f974" />

## Where:
- Calculator: Startup project responsible for managing individual implementations of calculators.
- Calculator.Persistance: Used for persistent history of calculations into local sqllite database.
- Calculator.Core: Lightway interface used to register implementations of calculator.
- Calculator.Standard: Implements default, standard calculator.

# How to add another calculator:
1. Create new, WPF library.
2. Reference "Calculator.Cre" project
3. Create view, class that implements `UserControl` and `ICalculatorWindow` interface.
4. Implement `ICalculatorWindow` interface, `Restore` method restores previously saved calculations, `Saved` event saves calculations.
5. Inside `App.xaml.cs` Register new calculator using extension method on `IServiceCollection`: `AddCalculator<T>(string name, string category, string icon, bool isDefault = false)` where generic argument is view.
   - Example: `services.AddCalculator<Calculator>("Standardowy", "Kalkulator", "🖩", true);`
