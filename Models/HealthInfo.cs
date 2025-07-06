using System;
using System.Collections.Generic;

namespace QuitSmartApp.Models;

public partial class HealthInfo
{
    public Guid InfoId { get; set; }

    public string Title { get; set; } = null!;

    public string Content { get; set; } = null!;

    public string InfoType { get; set; } = null!;

    public bool? IsActive { get; set; }

    public DateTime? CreatedAt { get; set; }
}
