using System;
using System.Collections.Generic;

namespace QuitSmartApp.Models;

public partial class UserBadgeCollection
{
    public Guid UserId { get; set; }

    public string FullName { get; set; } = null!;

    public string BadgeName { get; set; } = null!;

    public string? BadgeDescription { get; set; }

    public string? IconPath { get; set; }

    public string BadgeType { get; set; } = null!;

    public DateTime? EarnedAt { get; set; }
    
    // Computed properties for UI binding
    public bool IsEarned => EarnedAt.HasValue;
    
    // Map icon names to emojis
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
    
    public double Progress => IsEarned ? 100.0 : 0.0;
    
    public string EarnedDate => EarnedAt?.ToString("dd/MM/yyyy") ?? "";
    
    // Create a BadgeDefinition object for UI binding compatibility
    public BadgeDefinition BadgeDefinition => new()
    {
        Name = BadgeName,
        Description = BadgeDescription,
        IconPath = IconPath,
        BadgeType = BadgeType
    };
    
    // UserBadge property for compatibility
    public object? UserBadge => IsEarned ? new { } : null;
}
