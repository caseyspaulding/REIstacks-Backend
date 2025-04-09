//using Microsoft.Extensions.Configuration;
//using Microsoft.Extensions.Logging;
//using Newtonsoft.Json.Linq;
//using REIstacks.Application.Repositories.Interfaces;
//using REIstacks.Infrastructure.Data.FloridaParcels;


//namespace reistacks_api.Services.PropertyData;

//public class FloridaParcelService
//{
//    private readonly IFloridaParcelRepository _repository;
//    private readonly ILogger<FloridaParcelService> _logger;
//    private readonly HttpClient _httpClient;
//    private readonly BlobServiceClient _blobServiceClient;
//    private readonly string _blobContainerName;

//    public FloridaParcelService(
//        IFloridaParcelRepository repository,
//        ILogger<FloridaParcelService> logger,
//        HttpClient httpClient,
//        BlobServiceClient blobServiceClient,
//        IConfiguration configuration)
//    {
//        _repository = repository;
//        _logger = logger;
//        _httpClient = httpClient;
//        _blobServiceClient = blobServiceClient;
//        _blobContainerName = configuration["BlobStorage:FloridaParcelContainer"] ?? "florida-parcels";
//    }

//    public async Task ProcessCountyDataAsync(string county)
//    {
//        string url = $"https://services9.arcgis.com/Gh9awoU677aKree0/arcgis/rest/services/Florida_Statewide_Cadastral/FeatureServer/0/query?outFields=CO_NO,PARCEL_ID,JV,LND_VAL,NO_BULDNG,TOT_LVG_AR,SALE_PRC1,SALE_YR1,SALE_MO1,OWN_NAME,OWN_ADDR1,OWN_CITY,OWN_STATE,OWN_ZIPCD,S_LEGAL,PHY_ADDR1,PHY_CITY,PHY_ZIPCD,Shape__Area&where=CO_NO='{county}'&f=geojson";

//        try
//        {
//            // Download GeoJSON data
//            string jsonData = await DownloadGeoJsonAsync(url);

//            // Store raw backup in blob storage
//            await StoreRawDataAsync(jsonData, county);

//            // Process and store in SQL
//            await ProcessAndStoreInSqlAsync(jsonData);

//            _logger.LogInformation($"Successfully processed county {county}");
//        }
//        catch (Exception ex)
//        {
//            _logger.LogError(ex, $"Error processing county {county}");
//            throw;
//        }
//    }

//    private async Task<string> DownloadGeoJsonAsync(string url)
//    {
//        _logger.LogInformation($"Downloading GeoJSON from: {url}");

//        var response = await _httpClient.GetAsync(url);
//        response.EnsureSuccessStatusCode();

//        return await response.Content.ReadAsStringAsync();
//    }

//    private async Task StoreRawDataAsync(string jsonData, string county)
//    {
//        try
//        {
//            var containerClient = _blobServiceClient.GetBlobContainerClient(_blobContainerName);
//            await containerClient.CreateIfNotExistsAsync();

//            string fileName = $"{county}/{DateTime.UtcNow:yyyy-MM-dd}/raw-data.json";
//            var blobClient = containerClient.GetBlobClient(fileName);

//            using var stream = new System.IO.MemoryStream(System.Text.Encoding.UTF8.GetBytes(jsonData));
//            await blobClient.UploadAsync(stream, overwrite: true);

//            _logger.LogInformation($"Raw GeoJSON data for county {county} stored in blob storage");
//        }
//        catch (Exception ex)
//        {
//            _logger.LogError(ex, $"Error storing raw data for county {county}");
//            throw;
//        }
//    }

//    private async Task ProcessAndStoreInSqlAsync(string jsonData)
//    {
//        try
//        {
//            JObject geoJson = JObject.Parse(jsonData);
//            var features = geoJson["features"] as JArray;

//            if (features == null || !features.Any())
//            {
//                _logger.LogWarning("No features found in GeoJSON data");
//                return;
//            }

//            _logger.LogInformation($"Processing {features.Count} parcels");

//            var parcels = new List<FloridaParcel>();
//            var geometryByParcelId = new Dictionary<string, string>();

//            foreach (JObject feature in features)
//            {
//                var properties = feature["properties"] as JObject;
//                var geometry = feature["geometry"] as JObject;

//                if (properties == null || geometry == null)
//                {
//                    continue;
//                }

//                var parcel = MapToFloridaParcel(properties);
//                if (parcel != null)
//                {
//                    parcels.Add(parcel);
//                    geometryByParcelId[parcel.ParcelId] = geometry.ToString();
//                }
//            }

//            // Process in batches to avoid overwhelming the database
//            const int batchSize = 100;
//            for (int i = 0; i < parcels.Count; i += batchSize)
//            {
//                var batch = parcels.Skip(i).Take(batchSize);

//                // Filter the geometries dictionary to only include the current batch
//                var batchGeometries = new Dictionary<string, string>();
//                foreach (var parcel in batch)
//                {
//                    if (geometryByParcelId.TryGetValue(parcel.ParcelId, out var geo))
//                    {
//                        batchGeometries[parcel.ParcelId] = geo;
//                    }
//                }

//                await _repository.BatchInsertParcelsAsync(batch, batchGeometries);
//                _logger.LogInformation($"Inserted batch {i / batchSize + 1} of {(parcels.Count + batchSize - 1) / batchSize}");
//            }
//        }
//        catch (Exception ex)
//        {
//            _logger.LogError(ex, "Error processing and storing GeoJSON data");
//            throw;
//        }
//    }

//    private FloridaParcel MapToFloridaParcel(JObject properties)
//    {
//        try
//        {
//            return new FloridaParcel
//            {
//                ParcelId = properties["PARCEL_ID"]?.ToString(),
//                County = properties["CO_NO"]?.ToString(),
//                JustValue = ParseDecimal(properties["JV"]),
//                LandValue = ParseDecimal(properties["LND_VAL"]),
//                BuildingCount = ParseInt(properties["NO_BULDNG"]),
//                LivingArea = ParseInt(properties["TOT_LVG_AR"]),
//                LastSalePrice = ParseDecimal(properties["SALE_PRC1"]),
//                LastSaleYear = ParseInt(properties["SALE_YR1"]),
//                LastSaleMonth = properties["SALE_MO1"]?.ToString(),
//                OwnerName = properties["OWN_NAME"]?.ToString(),
//                OwnerAddress = properties["OWN_ADDR1"]?.ToString(),
//                OwnerCity = properties["OWN_CITY"]?.ToString(),
//                OwnerState = properties["OWN_STATE"]?.ToString(),
//                OwnerZip = properties["OWN_ZIPCD"]?.ToString(),
//                LegalDescription = properties["S_LEGAL"]?.ToString(),
//                PropertyAddress = properties["PHY_ADDR1"]?.ToString(),
//                PropertyCity = properties["PHY_CITY"]?.ToString(),
//                PropertyZip = properties["PHY_ZIPCD"]?.ToString(),
//                ParcelArea = ParseDecimal(properties["Shape__Area"])
//            };
//        }
//        catch (Exception ex)
//        {
//            _logger.LogWarning(ex, "Error mapping properties to FloridaParcel");
//            return null;
//        }
//    }

//    private decimal ParseDecimal(JToken token)
//    {
//        if (token == null) return 0;
//        return decimal.TryParse(token.ToString(), out var result) ? result : 0;
//    }

//    private int ParseInt(JToken token)
//    {
//        if (token == null) return 0;
//        return int.TryParse(token.ToString(), out var result) ? result : 0;
//    }

//    public static IEnumerable<string> GetFloridaCountyCodes()
//    {
//        // Florida county codes
//        return new string[]
//        {
//                "01", "02", "03", "04", "05", "06", "07", "08", "09", "10",
//                "11", "12", "13", "14", "15", "16", "17", "18", "19", "20",
//                "21", "22", "23", "24", "25", "26", "27", "28", "29", "30",
//                "31", "32", "33", "34", "35", "36", "37", "38", "39", "40",
//                "41", "42", "43", "44", "45", "46", "47", "48", "49", "50",
//                "51", "52", "53", "54", "55", "56", "57", "58", "59", "60",
//                "61", "62", "63", "64", "65", "66", "67"
//        };
//    }
//}
