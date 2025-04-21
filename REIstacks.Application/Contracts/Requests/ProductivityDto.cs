namespace REIstacks.Application.Contracts.Requests;
public class ProductivityDto
{
    public int NewLeadsThisWeek { get; set; }
    public int NewLeadsTarget { get; set; }

    public int LeadsContacted { get; set; }
    public int LeadsContactedTarget { get; set; }

    public int AppointmentsMade { get; set; }
    public int AppointmentsTarget { get; set; }

    public int OffersMade { get; set; }
    public int OffersTarget { get; set; }
}