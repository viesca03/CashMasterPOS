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

        public static void RunPOS()
        {
            Console.WriteLine("==============POS==============");
            Console.WriteLine("Enter the price of the item(s):");
            var price = double.Parse(Console.ReadLine());
            Console.WriteLine("Enter the payment (in the format denomination:quantity,denomination:quantity,...)");
            var inputPayment = Console.ReadLine();

            try
            {
                var payment = inputPayment.Split(',').ToDictionary(p => double.Parse(p.Split(':')[0]), p => int.Parse(p.Split(':')[1]));

                // Calculate the change
                GlobalService.LogService.Log("Transaction process start with Total: " + _mainSymbol + price.ToString() + " and paid with $" + payment.Sum(p => p.Key * p.Value).ToString());
                var change = GlobalService.ChangeCalculatorService.CalculateChange(price, payment);
                if (change.Count == 0)
                {
                    Console.WriteLine("ERROR: The total payment is less than the price. Please check and try again.");
                }
                else
                {
                    // Print the change calculation result and total change
                    Console.WriteLine("Change:");
                    var totalChange = GlobalService.ChangeCalculatorService.GetTotalChange(change, _mainSymbol);

                    GlobalService.LogService.Log("Total change of the transaction: " + _mainSymbol + totalChange);
                    Console.WriteLine("----------------------");
                    Console.WriteLine("TOTAL CHANGE: " + _mainSymbol + totalChange);
                    Console.WriteLine();

                    GlobalService.LogService.Log("End of a successful transaction.");
                }

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
                GlobalService.LogService.Log("Detailed Error: " + ex.Message);

                Console.WriteLine(error);
                Console.WriteLine();

                RunPOS();
            }
        }
    }
}
