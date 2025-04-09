

using Dapper;
using Microsoft.Data.SqlClient;
using Newtonsoft.Json.Linq;
using REIstacks.Application.Repositories.Interfaces;
using REIstacks.Infrastructure.Data.FloridaParcels;

namespace REIstacks.Infrastructure.Repositories.PropertyData;

public class FloridaParcelRepository : IFloridaParcelRepository
{
    private readonly string _connectionString;

    public FloridaParcelRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task<FloridaParcel> GetByIdAsync(string parcelId)
    {
        using (var connection = new SqlConnection(_connectionString))
        {
            await connection.OpenAsync();

            return await connection.QueryFirstOrDefaultAsync<FloridaParcel>(
                "SELECT * FROM FloridaParcels WHERE ParcelId = @ParcelId",
                new { ParcelId = parcelId });
        }
    }

    public async Task<IEnumerable<FloridaParcel>> GetByOwnerStateAsync(string state)
    {
        using (var connection = new SqlConnection(_connectionString))
        {
            await connection.OpenAsync();

            return await connection.QueryAsync<FloridaParcel>(
                "SELECT * FROM FloridaParcels WHERE OwnerState = @State",
                new { State = state });
        }
    }

    public async Task<IEnumerable<FloridaParcel>> GetNearLocationAsync(double lat, double lng, int radiusMeters)
    {
        using (var connection = new SqlConnection(_connectionString))
        {
            await connection.OpenAsync();

            string pointWkt = $"POINT({lng} {lat})";

            return await connection.QueryAsync<FloridaParcel>(@"
                SELECT *,
                       Geography.STDistance(geography::STPointFromText(@PointWkt, 4326)) AS DistanceMeters
                FROM FloridaParcels
                WHERE Geography.STDistance(geography::STPointFromText(@PointWkt, 4326)) <= @RadiusMeters
                ORDER BY Geography.STDistance(geography::STPointFromText(@PointWkt, 4326))",
                new { PointWkt = pointWkt, RadiusMeters = radiusMeters });
        }
    }

    public async Task UpsertParcelAsync(FloridaParcel parcel, string geoJson)
    {
        using (var connection = new SqlConnection(_connectionString))
        {
            await connection.OpenAsync();

            // Parse the geoJson to get geometry
            JObject geometry = JObject.Parse(geoJson);

            // Convert GeoJSON to WKT
            string wkt = ConvertGeoJsonToWkt(geometry);

            // Check if parcel exists
            var exists = await connection.ExecuteScalarAsync<int>(
                "SELECT COUNT(1) FROM FloridaParcels WHERE ParcelId = @ParcelId",
                new { parcel.ParcelId });

            if (exists > 0)
            {
                // Update existing
                await connection.ExecuteAsync(@"
                    UPDATE FloridaParcels SET
                        County = @County,
                        JustValue = @JustValue,
                        LandValue = @LandValue,
                        BuildingCount = @BuildingCount,
                        LivingArea = @LivingArea,
                        LastSalePrice = @LastSalePrice,
                        LastSaleYear = @LastSaleYear,
                        LastSaleMonth = @LastSaleMonth,
                        OwnerName = @OwnerName,
                        OwnerAddress = @OwnerAddress,
                        OwnerCity = @OwnerCity,
                        OwnerState = @OwnerState,
                        OwnerZip = @OwnerZip,
                        LegalDescription = @LegalDescription,
                        PropertyAddress = @PropertyAddress,
                        PropertyCity = @PropertyCity,
                        PropertyZip = @PropertyZip,
                        ParcelArea = @ParcelArea,
                        Geography = geography::STGeomFromText(@Wkt, 4326),
                        LastUpdated = GETUTCDATE()
                    WHERE ParcelId = @ParcelId",
                    new
                    {
                        parcel.ParcelId,
                        parcel.County,
                        parcel.JustValue,
                        parcel.LandValue,
                        parcel.BuildingCount,
                        parcel.LivingArea,
                        parcel.LastSalePrice,
                        parcel.LastSaleYear,
                        parcel.LastSaleMonth,
                        parcel.OwnerName,
                        parcel.OwnerAddress,
                        parcel.OwnerCity,
                        parcel.OwnerState,
                        parcel.OwnerZip,
                        parcel.LegalDescription,
                        parcel.PropertyAddress,
                        parcel.PropertyCity,
                        parcel.PropertyZip,
                        parcel.ParcelArea,
                        Wkt = wkt
                    });
            }
            else
            {
                // Insert new
                await connection.ExecuteAsync(@"
                    INSERT INTO FloridaParcels (
                        ParcelId, County, JustValue, LandValue, BuildingCount, 
                        LivingArea, LastSalePrice, LastSaleYear, LastSaleMonth, 
                        OwnerName, OwnerAddress, OwnerCity, OwnerState, OwnerZip,
                        LegalDescription, PropertyAddress, PropertyCity, PropertyZip, 
                        ParcelArea, Geography, LastUpdated)
                    VALUES (
                        @ParcelId, @County, @JustValue, @LandValue, @BuildingCount,
                        @LivingArea, @LastSalePrice, @LastSaleYear, @LastSaleMonth,
                        @OwnerName, @OwnerAddress, @OwnerCity, @OwnerState, @OwnerZip,
                        @LegalDescription, @PropertyAddress, @PropertyCity, @PropertyZip,
                        @ParcelArea, geography::STGeomFromText(@Wkt, 4326), GETUTCDATE())",
                    new
                    {
                        parcel.ParcelId,
                        parcel.County,
                        parcel.JustValue,
                        parcel.LandValue,
                        parcel.BuildingCount,
                        parcel.LivingArea,
                        parcel.LastSalePrice,
                        parcel.LastSaleYear,
                        parcel.LastSaleMonth,
                        parcel.OwnerName,
                        parcel.OwnerAddress,
                        parcel.OwnerCity,
                        parcel.OwnerState,
                        parcel.OwnerZip,
                        parcel.LegalDescription,
                        parcel.PropertyAddress,
                        parcel.PropertyCity,
                        parcel.PropertyZip,
                        parcel.ParcelArea,
                        Wkt = wkt
                    });
            }
        }
    }

    // Helper method to convert GeoJSON to WKT
    private string ConvertGeoJsonToWkt(JObject geometry)
    {
        string type = geometry["type"].ToString();
        JToken coordinates = geometry["coordinates"];

        if (type == "Point")
        {
            double x = coordinates[0].Value<double>();
            double y = coordinates[1].Value<double>();
            return $"POINT({x} {y})";
        }
        else if (type == "Polygon")
        {
            var outerRing = coordinates[0];
            var pointsList = new List<string>();

            foreach (var point in outerRing)
            {
                double x = point[0].Value<double>();
                double y = point[1].Value<double>();
                pointsList.Add($"{x} {y}");
            }

            string pointsText = string.Join(", ", pointsList);
            return $"POLYGON(({pointsText}))";
        }

        throw new NotSupportedException($"Geometry type '{type}' not supported");
    }

    // Additional helpful methods for wholesalers

    public async Task<IEnumerable<FloridaParcel>> GetOutOfStateOwnersAsync(string propertyState = "FL")
    {
        using (var connection = new SqlConnection(_connectionString))
        {
            await connection.OpenAsync();

            return await connection.QueryAsync<FloridaParcel>(@"
                SELECT * FROM FloridaParcels
                WHERE OwnerState != @PropertyState 
                  AND OwnerState IS NOT NULL
                  AND OwnerName IS NOT NULL",
                new { PropertyState = propertyState });
        }
    }

    public async Task<IEnumerable<FloridaParcel>> GetLongTermOwnersWithEquityAsync(int yearsOwned = 7)
    {
        using (var connection = new SqlConnection(_connectionString))
        {
            await connection.OpenAsync();

            int cutoffYear = DateTime.Now.Year - yearsOwned;

            return await connection.QueryAsync<FloridaParcel>(@"
                SELECT * FROM FloridaParcels
                WHERE LastSaleYear <= @CutoffYear
                  AND LastSalePrice > 0
                  AND JustValue > LastSalePrice * 1.3",
                new { CutoffYear = cutoffYear });
        }
    }

    public async Task<IEnumerable<FloridaParcel>> GetVacantLandParcelsAsync()
    {
        using (var connection = new SqlConnection(_connectionString))
        {
            await connection.OpenAsync();

            return await connection.QueryAsync<FloridaParcel>(@"
                SELECT * FROM FloridaParcels
                WHERE (BuildingCount = 0 OR BuildingCount IS NULL)
                  AND LivingArea = 0
                  AND ParcelArea > 0");
        }
    }

    public async Task BatchInsertParcelsAsync(IEnumerable<FloridaParcel> parcels,
                                             IDictionary<string, string> geometryByParcelId)
    {
        using (var connection = new SqlConnection(_connectionString))
        {
            await connection.OpenAsync();

            using (var transaction = connection.BeginTransaction())
            {
                try
                {
                    foreach (var parcel in parcels)
                    {
                        if (geometryByParcelId.TryGetValue(parcel.ParcelId, out string geoJson))
                        {
                            var geometry = JObject.Parse(geoJson);
                            string wkt = ConvertGeoJsonToWkt(geometry);

                            // Use same query as in UpsertParcelAsync but with transaction
                            await connection.ExecuteAsync(@"
                                MERGE FloridaParcels AS target
                                USING (VALUES (@ParcelId)) AS source (ParcelId)
                                ON target.ParcelId = source.ParcelId
                                WHEN MATCHED THEN
                                    UPDATE SET /* set fields same as above */
                                WHEN NOT MATCHED THEN
                                    INSERT /* insert fields same as above */",
                                new
                                {
                                    parcel.ParcelId,
                                    /* other parameters */
                                    Wkt = wkt
                                }, transaction);
                        }
                    }

                    transaction.Commit();
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }
    }
}