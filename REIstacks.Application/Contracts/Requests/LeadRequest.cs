namespace REIstacks.Application.Contracts.Requests
{
    public class LeadRequest
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? PreferredContactMethod { get; set; }

        // Legacy address fields
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? ZipCode { get; set; }

        // New property address fields
        public string? PropertyStreetAddress { get; set; }
        public string? PropertyCity { get; set; }
        public string? PropertyState { get; set; }
        public string? PropertyZipCode { get; set; }

        public string? PropertyType { get; set; }
        public string[] PropertyCondition { get; set; }
        public Dictionary<string, string> PropertyIssues { get; set; }
        public Dictionary<string, string> ReasonForSelling { get; set; }
        public string? TargetPrice { get; set; }
        public string[] Timeline { get; set; }
        public string? AdditionalInfo { get; set; }
        public bool ConsentTextMessages { get; set; }
        public bool ConsentPrivacyPolicy { get; set; }
        public int Step { get; set; }
        public string? Message { get; set; }
        public string? Source { get; set; }
    }
}