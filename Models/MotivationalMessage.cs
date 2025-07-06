using System;
using System.Collections.Generic;

namespace QuitSmartApp.Models;

public partial class MotivationalMessage
{
    public Guid MessageId { get; set; }

    public string? Title { get; set; }

    public string Content { get; set; } = null!;

    public string MessageType { get; set; } = null!;

    public bool? IsActive { get; set; }

    public DateTime? CreatedAt { get; set; }
}
