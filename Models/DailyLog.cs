using System;
using System.Collections.Generic;

namespace QuitSmartApp.Models;

public partial class DailyLog
{
    public Guid LogId { get; set; }

    public Guid UserId { get; set; }

    public DateOnly LogDate { get; set; }

    public bool? HasSmoked { get; set; }

    public string? HealthStatus { get; set; }

    public string? Notes { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual User User { get; set; } = null!;
}
