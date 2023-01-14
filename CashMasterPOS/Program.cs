using CashMasterPOS.Services;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CashMasterPOS
{
    internal class Program
    {
        private static readonly string _logFilePath = ConfigurationManager.AppSettings["LogFilePath"];
        private static readonly string _currentDenomination = ConfigurationManager.AppSettings["CurrentDenomination"];
        private static string _mainSymbol;

        private static readonly FileLoggerService _logService = new FileLoggerService(_logFilePath);
        private static readonly DenominationService _denominationService = new DenominationService(_currentDenomination);

        public static void Main(string[] args)
        {
            var currentDenomination = _denominationService.GetDenominations();
            var denominations = currentDenomination.Denominations.Select(d => d.Value).ToList();
            _mainSymbol = currentDenomination.MainSymbol;

            Run(denominations);
        }

        public static void Run(List<double> denominations)
        {
            var calculator = new ChangeCalculatorService(denominations, _logService);

            Console.WriteLine("==============POS==============");
            Console.WriteLine("Enter the price of the item(s):");
            var price = double.Parse(Console.ReadLine());
            Console.WriteLine("Enter the payment (in the format denomination:quantity,denomination:quantity,...)");
            var inputPayment = Console.ReadLine();

            try
            {
                var payment = inputPayment.Split(',').ToDictionary(p => double.Parse(p.Split(':')[0]), p => int.Parse(p.Split(':')[1]));

                // Calculate the change
                _logService.Log("Transaction process start with Total: " + _mainSymbol + price.ToString() + " and paid with $" + payment.Sum(p => p.Key * p.Value).ToString());
                var change = calculator.CalculateChange(price, payment);
                if (change.Item2)
                {
                    Console.WriteLine("ERROR: The total payment is less than the price. Please check and try again.");
                }
                else
                {
                    // Print the change
                    Console.WriteLine("Change:");
                    var totalChange = calculator.GetTotalChange(change.Item1, _mainSymbol);

                    _logService.Log("Total change of the transaction: " + _mainSymbol + totalChange);
                    Console.WriteLine("----------------------");
                    Console.WriteLine("TOTAL CHANGE: " + _mainSymbol + totalChange);
                    Console.WriteLine();

                    _logService.Log("End of a successful transaction.");
                }

                Console.WriteLine("Press ENTER for new transaction or press any other key to exit...");
                var restart = Console.ReadKey();
                if (restart.Key == ConsoleKey.Enter)
                {
                    Run(denominations);
                }
            }
            catch (Exception ex)
            {
                var error = "Error calculating change. Please check the input format and try again";
                _logService.Log(error);
                _logService.Log("Detailed Error: " + ex.Message);

                Console.WriteLine(error);
                Console.WriteLine();

                Run(denominations);
            }
        }
    }
}
