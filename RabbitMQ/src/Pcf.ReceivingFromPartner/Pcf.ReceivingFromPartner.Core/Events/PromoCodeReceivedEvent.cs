using System;

public class PromoCodeReceivedEvent
{
    public string PromoCode { get; set; }
    public Guid PartnerId { get; set; }
    public Guid? PartnerManagerId { get; set; }
    public DateTime IssuedDate { get; set; }
    public Guid PreferenceId { get; set; }
}
