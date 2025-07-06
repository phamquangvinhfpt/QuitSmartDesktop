using System;
using System.Collections.Generic;

namespace QuitSmartApp.Models;

public partial class AdminLog
{
    public Guid LogId { get; set; }

    public Guid AdminId { get; set; }

    public string Action { get; set; } = null!;

    public Guid? TargetUserId { get; set; }

    public string? Details { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual Admin Admin { get; set; } = null!;
}
