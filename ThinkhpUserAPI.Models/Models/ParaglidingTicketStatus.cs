using System;
using System.Collections.Generic;

namespace ThinkhpUserAPI.Models.Models;

public partial class ParaglidingTicketStatus
{
    public long TicketStatusId { get; set; }

    public long PurchasedTicketDetailId { get; set; }

    public bool? IsTicketPurchased { get; set; }

    public bool? IsTicketPrinted { get; set; }

    public bool? IsTicketScanned { get; set; }

    public virtual ParaglidingTicketPurchaseDetail PurchasedTicketDetail { get; set; } = null!;
}
