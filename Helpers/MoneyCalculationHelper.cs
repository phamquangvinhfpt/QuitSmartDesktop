using System;

namespace QuitSmartApp.Helpers
{
    // Money calculation utilities
    public static class MoneyCalculationHelper
    {
        // Calculate money saved based on days without smoking
        public static decimal CalculateMoneySaved(int daysQuit, int cigarettesPerDay, decimal pricePerPack, int cigarettesPerPack = 20)
        {
            if (daysQuit <= 0 || cigarettesPerDay <= 0 || pricePerPack <= 0 || cigarettesPerPack <= 0)
                return 0;

            var totalCigarettesAvoided = daysQuit * cigarettesPerDay;
            var pricePerCigarette = pricePerPack / cigarettesPerPack;
            
            return totalCigarettesAvoided * pricePerCigarette;
        }

        // Calculate potential future savings
        public static decimal CalculateFutureSavings(int futureDays, int cigarettesPerDay, decimal pricePerPack, int cigarettesPerPack = 20)
        {
            return CalculateMoneySaved(futureDays, cigarettesPerDay, pricePerPack, cigarettesPerPack);
        }

        // Format money amount to Vietnamese currency format
        public static string FormatMoney(decimal amount)
        {
            return amount.ToString("#,##0") + " đ";
        }
    }
}
