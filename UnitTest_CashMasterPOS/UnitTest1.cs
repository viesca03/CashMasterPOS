using CashMasterPOS.Interfaces;
using CashMasterPOS.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace UnitTest_CashMasterPOS
{
    [TestClass]
    public class UnitTest1
    {
        public UnitTest1()
        {
            var services = new ServiceCollection();
            services.AddTransient<ILogger, FileLoggerService>();
            services.AddTransient<IDenomination, DenominationService>();
            services.AddTransient<IChangeCalculator, ChangeCalculatorService>();

            var serviceProvider = services.BuildServiceProvider();

            GlobalService.LogService = serviceProvider.GetService<ILogger>();
            GlobalService.DenominationService = serviceProvider.GetService<IDenomination>();
            GlobalService.ChangeCalculatorService = serviceProvider.GetService<IChangeCalculator>();
        }

        [TestMethod]
        public void Should_Return_Correct_Change_When_Payment_Is_Greater_Than_Price()
        {
            var price = 750;

            //Payment = 1000
            var payment = new Dictionary<double, int> { { 500, 1 }, { 200, 2 }, { 100, 1 } };

            var change = GlobalService.ChangeCalculatorService.CalculateChange(price, payment);
            var expected = new Dictionary<double, int> 
            {
                { 500,0 },
                { 200,1 }, 
                { 100,0 },
                { 50,1 },
                { 20,0 },
                { 10,0 },
                { 5,0 },
                { 2,0 },
                { 1,0 },
                { 0.5,0 },
                { 0.2,0 },
                { 0.1,0 },
                { 0.05,0 },
            };

            Assert.AreEqual(ToAssertableString(expected), ToAssertableString(change));
        }

        public string ToAssertableString(IDictionary<double, int> dictionary)
        {
            var pairStrings = dictionary.OrderBy(p => p.Key)
                                        .Select(p => p.Key + ": " + string.Join(", ", p.Value));
            return string.Join("; ", pairStrings);
        }

        [TestMethod]
        public void Should_Return_Correct_Total_Change_Amount_When_Payment_Greater_Than_Price()
        {
            var price = 750;

            //Payment = 1000
            var payment = new Dictionary<double, int> { { 500, 1 }, { 200, 2 }, { 100, 1 } };

            var change = GlobalService.ChangeCalculatorService.CalculateChange(price, payment);
            var totalChange = GlobalService.ChangeCalculatorService.GetTotalChange(change, "$");
            var expected = "250";

            Assert.AreEqual(expected, totalChange);
        }

        [TestMethod]
        public void Should_Return_Correct_Total_Change_Amount_When_Payment_Equals_To_Price()
        {
            var price = 750;

            //Payment = 750
            var payment = new Dictionary<double, int> { { 500, 1 }, { 200, 1 }, { 50, 1 } };

            var change = GlobalService.ChangeCalculatorService.CalculateChange(price, payment);
            var totalChange = GlobalService.ChangeCalculatorService.GetTotalChange(change, "$");
            var expected = "0";

            Assert.AreEqual(expected, totalChange);
        }

        [TestMethod]
        public void Should_Return_Change_Count_0_When_Payment_Is_Less_Than_Price()
        {
            var price = 750;

            //Payment = 500
            var payment = new Dictionary<double, int> { { 200, 2 }, { 100, 1 } };

            var change = GlobalService.ChangeCalculatorService.CalculateChange(price, payment);
            var expected = 0;

            Assert.AreEqual(expected, change.Count);
        }
    }
}
