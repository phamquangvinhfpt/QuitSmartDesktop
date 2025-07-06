using System;
using System.Collections.Generic;

namespace QuitSmartApp.Models;

public partial class BadgeDefinition
{
    public Guid BadgeId { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public string? IconPath { get; set; }

    public string BadgeType { get; set; } = null!;

    public decimal RequiredValue { get; set; }

    public bool? IsActive { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual ICollection<UserBadge> UserBadges { get; set; } = new List<UserBadge>();
}
