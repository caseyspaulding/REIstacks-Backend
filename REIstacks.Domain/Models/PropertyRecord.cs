namespace REIstack.Domain.Models;

public class PropertyRecord
{
    public string Id { get; set; }
    public string FormattedAddress { get; set; }
    public string PropertyType { get; set; }
    public int Bedrooms { get; set; }
    public decimal Bathrooms { get; set; }
    public int SquareFootage { get; set; }
    public int LotSize { get; set; }
    public int YearBuilt { get; set; }
    public decimal LastSalePrice { get; set; }
    // Add other properties as needed
}
