using System;
using System.Collections.Generic;

namespace QuitSmartApp.Models;

public partial class UserStatistic
{
    public Guid StatId { get; set; }

    public Guid UserId { get; set; }

    public int? TotalDaysQuit { get; set; }

    public decimal? TotalMoneySaved { get; set; }

    public int? CurrentStreak { get; set; }

    public int? LongestStreak { get; set; }

    public DateTime? LastCalculatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual User User { get; set; } = null!;
}
