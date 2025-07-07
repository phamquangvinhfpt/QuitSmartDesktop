namespace QuitSmartApp.Configuration
{
    /// <summary>
    /// Application settings and configuration constants
    /// </summary>
    public static class AppSettings
    {
        // Database settings
        public const string DefaultConnectionString = "Server=localhost;Database=QuitSmartDesktop;User Id=sa;Password=12345;Trusted_Connection=True;TrustServerCertificate=True;";

        // UI Theme settings
        public const string PrimaryColor = "#4CAF50"; // Green color theme
        public const string SecondaryColor = "#2E7D32"; // Dark green
        public const string AccentColor = "#8BC34A"; // Light green

        // Application constants
        public const int DefaultCigarettesPerPack = 20;
        public const int MaxDailyLogsDisplay = 30;
        public const int MaxNotesLength = 255;

        // Badge thresholds
        public static readonly int[] DaysBadgeThresholds = { 1, 3, 7, 14, 30, 90, 180, 365 };
        public static readonly decimal[] MoneyBadgeThresholds = { 100000, 500000, 1000000, 5000000 };
        public static readonly int[] StreakBadgeThresholds = { 5, 10, 30 };

        // Health status options
        public static readonly string[] HealthStatusOptions = { "Good", "Average", "Poor" };

        // Message types
        public const string DailyMessageType = "Daily";
        public const string AchievementMessageType = "Achievement";
        public const string EncouragementMessageType = "Encouragement";

        // Health info types
        public const string SmokeEffectsType = "SmokeEffects";
        public const string QuitBenefitsType = "QuitBenefits";
        public const string TipsType = "Tips";
    }
}
