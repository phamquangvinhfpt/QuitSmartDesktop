using System;
using System.Collections.Generic;

namespace QuitSmartApp.Models;

public partial class UserBadge
{
    public Guid UserBadgeId { get; set; }

    public Guid UserId { get; set; }

    public Guid BadgeId { get; set; }

    public DateTime? EarnedAt { get; set; }

    public bool? IsNotified { get; set; }

    public virtual BadgeDefinition Badge { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
