using System;
using System.Collections.Generic;

namespace ThinkhpUserAPI.Models.Models;

public partial class ParaglidingTicketPurchaseDetail
{
    public long PurchasedTicketDetailId { get; set; }

    public long? PurchasedTicketId { get; set; }

    public long? UserId { get; set; }

    public decimal? NetAmount { get; set; }

    public virtual ICollection<ParaglidingTicketStatus> ParaglidingTicketStatuses { get; set; } = new List<ParaglidingTicketStatus>();

    public virtual ParaglidingTicketPurchase? PurchasedTicket { get; set; }

    public virtual User? User { get; set; }
}
