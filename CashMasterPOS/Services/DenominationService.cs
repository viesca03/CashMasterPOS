using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CashMasterPOS.Interfaces;
using CashMasterPOS.Models;
using Newtonsoft.Json;

namespace CashMasterPOS.Services
{
    public class DenominationService : IDenomination
    {
        private readonly string _country;

        public DenominationService()
        {
            _country = ConfigurationManager.AppSettings["CurrentDenomination"];
        }

        /// <summary>
        /// Method to get all denominations from the JSON file according the current country used on the App.config file
        /// </summary>
        /// <returns>A DenominationRootModel object which contains the list of all the country bills and coins denominations.</returns>
        public DenominationRootModel GetDenominations()
        {
            var filePath = @"Denominations/" + _country + ".json";
            var json = File.ReadAllText(filePath);
            var result = JsonConvert.DeserializeObject<DenominationRootModel>(json);
            return result;
        }

        public List<decimal> CheckIfDenimonationError(List<decimal> payment)
        {
            var denominations = GetDenominations().Denominations.Select(d => d.Value).ToList();

            var denominationDifference = payment.Where(p => !denominations.Contains(p)).ToList();
            
            return denominationDifference;
        }
    }
}
