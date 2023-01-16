using CashMasterPOS.Interfaces;
using CashMasterPOS.Services;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CashMasterPOS.Services
{
    public class ChangeCalculatorService : IChangeCalculator
    {
        public Dictionary<double, int> CalculateChange(double price, Dictionary<double, int> payment)
        {
            var result = new Dictionary<double, int>();

            var denominations = GlobalService.DenominationService.GetDenominations().Denominations.Select(d => d.Value).ToList();

            //Verify that the total payment is greater than or equal to the price
            double totalPayment = payment.Sum(p => p.Key * p.Value);

            if (totalPayment < price)
            {
                var error = "The total payment is less than the price";
                GlobalService.LogService.Log("Transaction Error: " + error);
                return result;
            }

            //Calculate change due
            GlobalService.LogService.Log("Calculating the optimal change amount...");
            double changeAmount = Math.Round((totalPayment - price), 2);
            denominations.Sort((a, b) => -1 * a.CompareTo(b));

            var change = denominations.ToDictionary(d => d, d => 0);

            //Iterate through the denominationValues in descending order
            foreach (var denomination in denominations)
            {
                change[denomination] = (int)(changeAmount / denomination);
                changeAmount -= change[denomination] * denomination;
            }

            result = change;

            return result;
        }

        public string GetTotalChange(Dictionary<double, int> change, string mainSymbol)
        {
            //Iterate through the change result to sum values greater than 0 and get total change
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
