using CashMasterPOS.Services;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CashMasterPOS.Services
{
    public class ChangeCalculatorService
    {
        private readonly List<double> _denominations;
        private readonly FileLoggerService _logService;
        

        public ChangeCalculatorService(List<double> denominations, FileLoggerService logService)
        {
            _denominations = denominations;
            _logService = logService;
        }

        public Tuple<Dictionary<double, int>, bool> CalculateChange(double price, Dictionary<double, int> payment)
        {
            //Verify that the total payment is greater than or equal to the price
            double totalPayment = payment.Sum(p => p.Key * p.Value);

            if (totalPayment < price)
            {
                var error = "The total payment is less than the price";
                _logService.Log("Transaction Error: " + error);
                return new Tuple<Dictionary<double, int>, bool>(null, true);
            }

            //Calculate change due
            _logService.Log("Calculating the optimal change amount...");
            double changeAmount = Math.Round((totalPayment - price), 2);
            _denominations.Sort((a, b) => -1 * a.CompareTo(b));

            var change = _denominations.ToDictionary(d => d, d => 0);

            //Iterate through the denominationValues in descending order
            foreach (var denomination in _denominations)
            {
                change[denomination] = (int)(changeAmount / denomination);
                changeAmount -= change[denomination] * denomination;
            }

            var result = new Tuple<Dictionary<double, int>, bool>(change, false);

            return result;
        }

        public string GetTotalChange(Dictionary<double, int> change, string mainSymbol)
        {
            foreach (var denomination in change)
            {
                if (denomination.Value != 0)
                {
                    Console.WriteLine(mainSymbol + denomination.Key + ":" + denomination.Value + " = " + mainSymbol + (denomination.Key * denomination.Value));
                }
            }

            var totalChange = change.Sum(c => c.Key * c.Value).ToString();

            return totalChange;
        }
    }
}
