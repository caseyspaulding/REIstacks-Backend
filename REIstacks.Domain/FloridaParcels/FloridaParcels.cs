namespace REIstacks.Infrastructure.Data.FloridaParcels;

public class FloridaParcel
{
    public string ParcelId { get; set; }
    public string County { get; set; }
    public decimal? JustValue { get; set; }
    public decimal? LandValue { get; set; }
    public int? BuildingCount { get; set; }
    public int? LivingArea { get; set; }
    public decimal? LastSalePrice { get; set; }
    public int? LastSaleYear { get; set; }
    public string LastSaleMonth { get; set; }
    public string OwnerName { get; set; }
    public string OwnerAddress { get; set; }
    public string OwnerCity { get; set; }
    public string OwnerState { get; set; }
    public string OwnerZip { get; set; }
    public string LegalDescription { get; set; }
    public string PropertyAddress { get; set; }
    public string PropertyCity { get; set; }
    public string PropertyZip { get; set; }
    public decimal? ParcelArea { get; set; }
    public DateTime LastUpdated { get; set; }

    // Used for distance calculations from spatial queries
    public double? DistanceMeters { get; set; }
}