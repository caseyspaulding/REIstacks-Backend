namespace REIstacks.Domain.Entities.CRM;
public class PropertyFilterCriteria
{
    // Property characteristics
    public string[] PropertyTypes { get; set; }
    public int? MinBedrooms { get; set; }
    public int? MaxBedrooms { get; set; }
    public int? MinBathrooms { get; set; }
    public int? MaxBathrooms { get; set; }
    public int? MinSquareFootage { get; set; }
    public int? MaxSquareFootage { get; set; }
    public int? MinYearBuilt { get; set; }
    public int? MaxYearBuilt { get; set; }

    // Financial criteria
    public decimal? MinEstimatedARV { get; set; }
    public decimal? MaxEstimatedARV { get; set; }
    public decimal? MinSellerAskingPrice { get; set; }
    public decimal? MaxSellerAskingPrice { get; set; }
    public decimal? MinEquity { get; set; }
    public decimal? MaxEquity { get; set; }

    // Ownership criteria
    public bool? IsAbsenteeOwner { get; set; }
    public bool? IsOwnerOccupied { get; set; }
    public bool? IsVacant { get; set; }
    public bool? IsDeceased { get; set; }
    public bool? IsFreeAndClear { get; set; }
    public bool? IsHighEquity { get; set; }
    public bool? IsLowEquity { get; set; }
    public bool? IsNegativeEquity { get; set; }

    // Property status
    public bool? IsForeclosure { get; set; }
    public bool? IsPreForeclosure { get; set; }
    public bool? IsTaxLien { get; set; }
    public bool? IsBankOwned { get; set; }
    public bool? IsAuction { get; set; }
    public bool? IsJudgment { get; set; }

    // Loan information
    public bool? IsAssumableLoan { get; set; }
    public bool? IsAdjustableLoan { get; set; }
    public bool? IsPrivateLender { get; set; }

    // Transaction history
    public bool? IsQuickResale { get; set; }
    public bool? IsIntrafamilyTransfer { get; set; }
    public bool? IsCashBuyer { get; set; }

    // Geographic criteria
    public string City { get; set; }
    public string State { get; set; }
    public string County { get; set; }
    public string ZipCode { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public double? RadiusMiles { get; set; }
}