using Calculator.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static System.Net.Mime.MediaTypeNames;

namespace Calculator.Standard
{
    /// <summary>
    /// Interaction logic for Calculator.xaml
    /// </summary>
    public partial class Calculator : UserControl, ICalculatorWindow
    {
        public Calculator()
        {
            InitializeComponent();
        }

        public event SavedCallback? Saved;
        public void Restore(string input, string output)
        {
            Input.Content = input;
            Output.Content = output;
        }

        private void Input_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            
            var value = button.Tag.ToString();
            switch (value)
            {
                case "C": // Clear
                    Input.Content = "";
                    Output.Content = "0";
                    break;
                case "=": // Calculate
                    {
                        var content = (string)Input.Content;
                        var expression = new NCalc.Expression(content);
                        try
                        {
                            var result = expression.Evaluate();
                            Output.Content = result.ToString();
                            Saved?.Invoke((string)Input.Content, (string)Output.Content);
                        }
                        catch (Exception)
                        {
                            Output.Content = "Nieprawidłowe dane wejściowe";
                        }
                    }
                    break;
                default:
                    {
                        var content = (string)Input.Content;
                        if(content.Length > 0 && value == "/" || value == "*" || value == "-" || value == "+")
                        {
                            switch (content[^1])
                            {
                                case '/':
                                case '*':
                                case '-':
                                case '+':
                                    content = content[..^1];
                                    break;
                            }
                        }
                    
                        Input.Content = $"{content}{value}";
                    }
                    break;
            }
            // Use value here
        }
    }
}
