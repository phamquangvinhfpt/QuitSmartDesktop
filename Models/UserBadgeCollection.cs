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
}
