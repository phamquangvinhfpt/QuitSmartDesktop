using System;
using System.Collections.Generic;

namespace QuitSmartApp.Models;

public partial class HealthTrackingOverview
{
    public Guid UserId { get; set; }

    public string FullName { get; set; } = null!;

    public DateOnly LogDate { get; set; }

    public bool? HasSmoked { get; set; }

    public string? HealthStatus { get; set; }

    public string? Notes { get; set; }

    public int? CurrentStreak { get; set; }

    public int? TotalDaysQuit { get; set; }
}
