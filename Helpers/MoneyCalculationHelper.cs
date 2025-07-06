using System;

namespace QuitSmartApp.Helpers
{
    /// <summary>
    /// Money calculation utilities for quit smoking savings
    /// </summary>
    public static class MoneyCalculationHelper
    {
        /// <summary>
        /// Calculate money saved based on days without smoking
        /// </summary>
        public static decimal CalculateMoneySaved(int daysQuit, int cigarettesPerDay, decimal pricePerPack, int cigarettesPerPack = 20)
        {
            if (daysQuit <= 0 || cigarettesPerDay <= 0 || pricePerPack <= 0 || cigarettesPerPack <= 0)
                return 0;

            var totalCigarettesAvoided = daysQuit * cigarettesPerDay;
            var pricePerCigarette = pricePerPack / cigarettesPerPack;
            
            return totalCigarettesAvoided * pricePerCigarette;
        }

        /// <summary>
        /// Calculate potential future savings
        /// </summary>
        public static decimal CalculateFutureSavings(int futureDays, int cigarettesPerDay, decimal pricePerPack, int cigarettesPerPack = 20)
        {
            return CalculateMoneySaved(futureDays, cigarettesPerDay, pricePerPack, cigarettesPerPack);
        }

        /// <summary>
        /// Format money amount to Vietnamese currency format
        /// </summary>
        public static string FormatMoney(decimal amount)
        {
            return amount.ToString("#,##0") + " đ";
        }
    }
}
