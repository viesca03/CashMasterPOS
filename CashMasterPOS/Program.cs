using CashMasterPOS.Services;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CashMasterPOS.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace CashMasterPOS
{
     public class Program
    {
        private static string _mainSymbol;

        public static void Main(string[] args)
        {
            var services = new ServiceCollection();
            services.AddTransient<ILogger, FileLoggerService>();
            services.AddTransient<IDenomination, DenominationService>();
            services.AddTransient<IChangeCalculator, ChangeCalculatorService>();

            var serviceProvider = services.BuildServiceProvider();

            GlobalService.LogService = serviceProvider.GetService<ILogger>();
            GlobalService.DenominationService = serviceProvider.GetService<IDenomination>();
            GlobalService.ChangeCalculatorService = serviceProvider.GetService<IChangeCalculator>();


            var currentDenomination = GlobalService.DenominationService.GetDenominations();
            _mainSymbol = currentDenomination.MainSymbol;

            RunPOS();
        }

        /// <summary>
        /// A method to run and restart the user interface when the process finish or an error is found
        /// </summary>

        public static void RunPOS()
        {
            Console.WriteLine();
            Console.WriteLine("==============POS==============");
            Console.WriteLine("Enter the price of the item(s):");

            if (!decimal.TryParse(Console.ReadLine(), out decimal price))
            {
                Console.WriteLine("Invalid price, enter only numbers.");
                RunPOS();
            }

            Console.WriteLine("Enter the payment (in the format denomination:quantity,denomination:quantity,...)");
            var inputPayment = Console.ReadLine();

            try
            {
                var payment = inputPayment.Split(',').ToDictionary(p => decimal.Parse(p.Split(':')[0]), p => int.Parse(p.Split(':')[1]));

                //Check if all denominations in input are in the JSON denominations file
                var denominationError = GlobalService.DenominationService.CheckIfDenimonationError(payment.Select(p => p.Key).ToList());
                if (denominationError.Count != 0)
                {
                    var error = $"Denomination(s) {string.Join(",", denominationError)} not found in current country denomination. Use a different denomination to pay or change the denominations json file";
                    GlobalService.LogService.Log(error);
                    Console.WriteLine(error);
                }
                else
                {
                    // Calculate the change
                    GlobalService.LogService.Log($"Transaction process start with Total: {_mainSymbol}{price} and paid with {_mainSymbol} {payment.Sum(p => p.Key * p.Value)}");
                    var isValidTransaction = GlobalService.ChangeCalculatorService.IsValidTransaction(price, payment);

                    if (!isValidTransaction)
                    {
                        Console.WriteLine("ERROR: The total payment is less than the price. Please check and try again.");
                    }
                    else
                    {
                        var change = GlobalService.ChangeCalculatorService.CalculateChange(price, payment);

                        // Print the change calculation result and total change
                        var totalChange = GlobalService.ChangeCalculatorService.GetTotalChange(change, _mainSymbol);

                        var difference = GlobalService.ChangeCalculatorService.GetChangeDifference(price, payment, totalChange);

                        GlobalService.LogService.Log($"Total change of the transaction: {_mainSymbol}{totalChange}");
                        Console.WriteLine("----------------------");
                        Console.WriteLine($"TOTAL CHANGE: {_mainSymbol}{totalChange}");
                        Console.WriteLine();

                        if (difference > 0)
                        {
                            var warning = $"The current denomination list doesn't have the capability to deliver exact change. \nPending Change: {difference}";
                            Console.WriteLine(warning);
                            GlobalService.LogService.Log("End of a successful transaction. With amount difference warning.");
                        }
                        else
                        {
                            GlobalService.LogService.Log("End of a successful transaction.");
                        }
                    }
                }

                Console.WriteLine();
                Console.WriteLine("Press ENTER for new transaction or press any other key to exit...");
                var restart = Console.ReadKey();
                if (restart.Key == ConsoleKey.Enter)
                {
                    RunPOS();
                }
            }
            catch (Exception ex)
            {
                var error = "Error calculating change. Please check the input format and try again";
                GlobalService.LogService.Log(error);
                GlobalService.LogService.Log($"Detailed Error: {ex.Message}");

                Console.WriteLine(error);
                Console.WriteLine();

                RunPOS();
            }
        }
    }
}
