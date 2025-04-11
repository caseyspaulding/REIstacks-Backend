namespace REIstacks.Domain.Entities.Properties;

public class ValueEstimate
{
    public int Price { get; set; }
    public int PriceRangeLow { get; set; }
    public int PriceRangeHigh { get; set; }
    public List<PropertyRecord> Comparables { get; set; }
}