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
        /// <summary>
        /// Method to get the minimum change to return to the client
        /// </summary>
        /// <param name="price">The total amount paid by the client</param>
        /// <param name="payment">The value and the amount of bills provided by the client</param>
        /// <returns>A list of decimal amounts showing the count of each denomination to return to the client</returns>
        public List<decimal> CalculateChange(decimal price, Dictionary<decimal, int> payment)
        {
            var result = new List<decimal>();

            var denominations = GlobalService.DenominationService.GetDenominations().Denominations.Select(d => d.Value).ToList();

            //Verify that the total payment is greater than or equal to the price
            decimal totalPayment = payment.Sum(p => p.Key * p.Value);

            if (totalPayment < price)
            {
                var error = "The total payment is less than the price";
                GlobalService.LogService.Log("Transaction Error: " + error);
                return result;
            }

            //Calculate change due
            GlobalService.LogService.Log("Calculating the optimal change amount...");
            decimal changeAmount = Math.Round(totalPayment - price, 2);
            denominations.Sort((a, b) => -1 * a.CompareTo(b));

            //Iterate through the denominations in descending order to get the optimum result
            result = PerformCalculation(denominations, result, changeAmount);

            return result;
        }

        /// <summary>
        /// Recursive function to take the first item of the denomination list and compare it against the change amount which will
        /// be decreased until 0 to know the best and minimal change to deliver to the client
        /// </summary>
        /// <param name="denominations">A decimal list with all the current country denominations</param>
        /// <param name="change">An empty decimal list (in first iteration) that will contain the denominations to deliver to the client. If the list contains repeated elements, that means that you will be delivering more than one bill or coin of that denomination</param>
        /// <param name="changeAmount">The calculated total change to deliver to the client</param>
        /// <returns></returns>
        private List<decimal> PerformCalculation(List<decimal> denominations, List<decimal> change, decimal changeAmount)
        {
            var denomination = denominations.FirstOrDefault();
            if (denominations.Count == 0 && changeAmount != 0)
            {
                var msg = "The current denomination list doesn't have the capability to deliver exact change. \n" + "Pending Change: " + changeAmount;

                Console.WriteLine();
                Console.WriteLine(msg);
                Console.WriteLine("-----------------------------");

                GlobalService.LogService.Log(msg);
                return change;
            }
            if (changeAmount >= denomination && denominations.Count != 0)
            {
                changeAmount = decimal.Subtract(changeAmount, denomination);

                change.Add(denomination);
                if (changeAmount != 0)
                {
                    PerformCalculation(denominations, change, changeAmount);
                }
            }
            if (changeAmount == 0 && denominations.Count == 0)
            {
                return change;
            }
            else
            {
                denominations.Remove(denomination);
                PerformCalculation(denominations, change, changeAmount);
            }

            return change;
        }
        /// <summary>
        /// Method to calculate and show the simplified total change amount to return to the client
        /// </summary>
        /// <param name="change">A list of decimals with the minimal change provided by the CalculateChange method </param>
        /// <param name="mainSymbol">The currency symbol of the current country</param>
        /// <returns>A string with the total change to return to the client</returns>
        public string GetTotalChange(List<decimal> change, string mainSymbol)
        {
            //Iterate through the change result to sum values greater than 0 and get total change
            var groupedChange = change.GroupBy(c => c).ToDictionary(g => g.Key, g => g.Count());
            foreach (var gc in groupedChange)
            {
                if (gc.Value != 0)
                {
                    Console.WriteLine(mainSymbol + gc.Key + ":" + gc.Value + " = " + mainSymbol + (gc.Key * gc.Value));
                }
            }

            var totalChange = change.Sum(c => c).ToString();

            return totalChange;
        }
    }
}
