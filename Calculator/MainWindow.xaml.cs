using Calculator.Core;
using Calculator.Persistence;
using Microsoft.Extensions.DependencyInjection;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Kalkulator;

public partial class MainWindow : Window, INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    private readonly Calculators _calculators;
    private readonly IServiceProvider _serviceProvider;
    private readonly SavedEntriesRepository _savedEntriesRepository;

    public IReadOnlyDictionary<string, CalculatorInstance[]> CalculatorsByType => _calculators.CalculatorsByType;

    private Tuple<string, string>[] _savedEntries;
    public Tuple<string, string>[] SavedEntries
    {
        get => _savedEntries;
        set
        {
            if (_savedEntries == value)
                return;

            _savedEntries = value;
            OnPropertyChanged();
        }
    }

    private CalculatorInstance? _selected;
    public CalculatorInstance? Selected
    {
        get => _selected;
        set
        {
            if (_selected == value)
                return;

            _selected = value;
            SelectCalculator(value!);
            OnPropertyChanged();
        }
    }
    public MainWindow(Calculators calculators, IServiceProvider serviceProvider, SavedEntriesRepository savedEntriesRepository)
    {
        _calculators = calculators;
        _serviceProvider = serviceProvider;
        _savedEntriesRepository = savedEntriesRepository;
        InitializeComponent();
        Loaded += HandleLoaded;
        DataContext = this;
    }

    private void HandleLoaded(object sender, RoutedEventArgs e)
    {
        Selected = _calculators.Default;
    }

    private void DockPanel_MouseDown(object sender, MouseButtonEventArgs e)
    {
        if (e.ChangedButton == MouseButton.Left)
        {
            DragMove();
        }
    }

    private void Close_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }

    private void SelectCalculator(CalculatorInstance calculatorInstance)
    {
        var calculatorUserControl = (UserControl)ActivatorUtilities.CreateInstance(_serviceProvider, calculatorInstance.UserControlType);
        foreach (var item in Content.Children)
        {
            if (item is ICalculatorWindow itemCalculatorWindow)
            {
                itemCalculatorWindow.Saved -= HandleSaved;
            }
        }
        Content.Children.Clear();

        if (calculatorUserControl is ICalculatorWindow calculatorWindow)
        {
            calculatorWindow.Saved += HandleSaved;
        }
        calculatorUserControl.Width = Content.ActualWidth;
        calculatorUserControl.Height = Content.ActualHeight;
        Content.Children.Add(calculatorUserControl);
    }

    private void HandleSaved(string input, string output)
    {
        if (Selected == null)
            throw new InvalidOperationException();

        Task.Run(async () =>
        {
            await _savedEntriesRepository.Add($"{Selected.Category}/{Selected.Name}",input, output);
        });
    }

    private void MenuItem_Click(object sender, MouseButtonEventArgs e)
    {
        var calculatorInstance = (CalculatorInstance)((Grid)sender).DataContext;
        Selected = calculatorInstance;
        Menu.Visibility = Visibility.Collapsed;
    }
    private void Minimize_Click(object sender, RoutedEventArgs e)
    {
        WindowState = WindowState.Minimized;
    }

    private void Maximize_Click(object sender, RoutedEventArgs e)
    {
        if (WindowState == WindowState.Maximized)
            WindowState = WindowState.Normal;
        else
            WindowState = WindowState.Maximized;
    }

    private void ExpandCollapseMenu_Click(object sender, RoutedEventArgs e)
    {
        if (Menu.Visibility == Visibility.Visible)
            Menu.Visibility = Visibility.Collapsed;
        else
            Menu.Visibility = Visibility.Visible;
    }

    private async void OpenHistory_Click(object sender, RoutedEventArgs e)
    {
        if (Selected == null)
            throw new InvalidOperationException();

        SavedEntries = await _savedEntriesRepository.GetAll($"{Selected.Category}/{Selected.Name}");

        if (History.Visibility == Visibility.Visible)
            History.Visibility = Visibility.Collapsed;
        else
            History.Visibility = Visibility.Visible;
    }

    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private void SavedEntryRestore_Click(object sender, RoutedEventArgs e)
    {
        if(sender is ListBox listBox && listBox.SelectedItem is Tuple<string, string> entry)
        {
            foreach (var item in Content.Children)
            {
                if (item is ICalculatorWindow itemCalculatorWindow)
                {
                    itemCalculatorWindow.Restore(entry.Item1, entry.Item2);
                }
            }
        }
        History.Visibility = Visibility.Collapsed;
    }

    private async void Clear_Click(object sender, RoutedEventArgs e)
    {
        if (Selected == null)
            throw new InvalidOperationException();

        await _savedEntriesRepository.Clear($"{Selected.Category}/{Selected.Name}");
        SavedEntries = [];

        History.Visibility = Visibility.Collapsed;
    }
}