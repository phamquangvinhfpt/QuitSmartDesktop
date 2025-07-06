using System;
using System.Collections.Generic;

namespace QuitSmartApp.Models;

public partial class UserProfile
{
    public Guid ProfileId { get; set; }

    public Guid UserId { get; set; }

    public DateOnly QuitStartDate { get; set; }

    public DateOnly? QuitGoalDate { get; set; }

    public int CigarettesPerDay { get; set; }

    public decimal PricePerPack { get; set; }

    public int? CigarettesPerPack { get; set; }

    public int? SmokingYears { get; set; }

    public string? QuitReason { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual User User { get; set; } = null!;
}
