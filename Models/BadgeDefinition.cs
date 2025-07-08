using System;
using System.Collections.Generic;

namespace QuitSmartApp.Models;

public partial class BadgeDefinition
{
    public Guid BadgeId { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public string? IconPath { get; set; }

    // Computed property for UI binding - maps icon names to emojis
    public string Icon => GetIconEmoji(IconPath ?? "default");

    private static string GetIconEmoji(string iconName) => iconName.ToLower() switch
    {
        "first-day" or "first_day" => "🌟",
        "3-days" or "3_days" => "💪",
        "one-week" or "one_week" => "🏃‍♂️",
        "two-weeks" or "two_weeks" => "⭐",
        "one-month" or "one_month" => "🏆",
        "three-months" or "three_months" => "👑",
        "half-year" or "half_year" => "💎",
        "one-year" or "one_year" => "🥇",
        "saver-100k" or "saver_100k" => "💰",
        "saver-500k" or "saver_500k" => "💸",
        "saver-1m" or "saver_1m" => "🤑",
        "saver-5m" or "saver_5m" => "💵",
        "streak-5" or "streak_5" => "🔥",
        "streak-10" or "streak_10" => "⚡",
        "streak-30" or "streak_30" => "🚀",
        "health" => "❤️",
        "milestone" => "🏆",
        "money" => "💰",
        "days" => "📅",
        _ => "🏅" // default fallback
    };

    public string BadgeType { get; set; } = null!;

    public decimal RequiredValue { get; set; }

    public bool? IsActive { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual ICollection<UserBadge> UserBadges { get; set; } = new List<UserBadge>();
}
