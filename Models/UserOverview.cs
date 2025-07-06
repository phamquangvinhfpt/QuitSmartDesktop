using System;
using System.Collections.Generic;

namespace QuitSmartApp.Models;

public partial class UserOverview
{
    public Guid UserId { get; set; }

    public string Username { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string FullName { get; set; } = null!;

    public string? Gender { get; set; }

    public bool? IsActive { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateOnly? QuitStartDate { get; set; }

    public int? CigarettesPerDay { get; set; }

    public decimal? PricePerPack { get; set; }

    public string? QuitReason { get; set; }

    public int? TotalDaysQuit { get; set; }

    public decimal? TotalMoneySaved { get; set; }

    public int? CurrentStreak { get; set; }

    public int? LongestStreak { get; set; }

    public int? TotalBadges { get; set; }
}
