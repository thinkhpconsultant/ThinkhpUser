using System;
using System.Collections.Generic;

namespace ThinkhpUserAPI.Models.Models;

public partial class ParaglidingTicketPurchase
{
    public long PurchasedTicketId { get; set; }

    public long? UserId { get; set; }

    public int? NoOfTicket { get; set; }

    public decimal? AmountPerTicket { get; set; }

    public decimal? DiscountPercentage { get; set; }

    public decimal? DiscountAmount { get; set; }

    public decimal? Cgstpercentage { get; set; }

    public decimal? Cgstamount { get; set; }

    public decimal? Sgstpercentage { get; set; }

    public decimal? Sgstamount { get; set; }

    public decimal? NetAmount { get; set; }

    public virtual ICollection<ParaglidingTicketPurchaseDetail> ParaglidingTicketPurchaseDetails { get; set; } = new List<ParaglidingTicketPurchaseDetail>();

    public virtual User? User { get; set; }
}
