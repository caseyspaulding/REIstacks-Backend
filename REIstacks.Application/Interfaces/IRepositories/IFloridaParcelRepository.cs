using REIstacks.Infrastructure.Data.FloridaParcels;

namespace REIstacks.Application.Repositories.Interfaces
{
    public interface IFloridaParcelRepository
    {
        Task<FloridaParcel> GetByIdAsync(string parcelId);
        Task<IEnumerable<FloridaParcel>> GetByOwnerStateAsync(string state);
        Task<IEnumerable<FloridaParcel>> GetNearLocationAsync(double lat, double lng, int radiusMeters);
        Task UpsertParcelAsync(FloridaParcel parcel, string geoJson);
        Task<IEnumerable<FloridaParcel>> GetOutOfStateOwnersAsync(string propertyState = "FL");
        Task<IEnumerable<FloridaParcel>> GetLongTermOwnersWithEquityAsync(int yearsOwned = 7);
        Task<IEnumerable<FloridaParcel>> GetVacantLandParcelsAsync();
        Task BatchInsertParcelsAsync(IEnumerable<FloridaParcel> parcels, IDictionary<string, string> geometryByParcelId);
    }
}