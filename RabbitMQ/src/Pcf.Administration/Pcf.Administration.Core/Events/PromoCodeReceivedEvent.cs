using System;

public class PromoCodeReceivedEvent
{
    public string PromoCode { get; set; }
    public int UserId { get; set; }
    public DateTime ReceivedDate { get; set; }
}
