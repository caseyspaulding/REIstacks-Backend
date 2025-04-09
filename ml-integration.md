# ReiStacks ML Integration Architecture

## Overview

The ML Integration layer in ReiStacks provides AI and machine learning capabilities for property valuation, lead scoring, and document analysis. This document outlines the architecture and implementation approach.

## Architecture Diagram

```
┌───────────────────┐     ┌───────────────────┐     ┌───────────────────┐
│  Application      │     │  ML.Integration   │     │  Azure Services   │
│                   │     │                   │     │                   │
│  ┌─────────────┐  │     │  ┌─────────────┐  │     │  ┌─────────────┐  │
│  │ IProperty   │  │     │  │ Property    │  │     │  │ Azure       │  │
│  │ Valuation   │◄─┼─────┼──┤ Valuation   │◄─┼─────┼──┤ OpenAI      │  │
│  │ Service     │  │     │  │ Service     │  │     │  │             │  │
│  └─────────────┘  │     │  └─────────────┘  │     │  └─────────────┘  │
│                   │     │                   │     │                   │
│  ┌─────────────┐  │     │  ┌─────────────┐  │     │  ┌─────────────┐  │
│  │ ILeadScoring│  │     │  │ LeadScoring │  │     │  │ Azure       │  │
│  │ Service     │◄─┼─────┼──┤ Service     │◄─┼─────┼──┤ Cognitive   │  │
│  │             │  │     │  │             │  │     │  │ Services    │  │
│  └─────────────┘  │     │  └─────────────┘  │     │  └─────────────┘  │
│                   │     │                   │     │                   │
│  ┌─────────────┐  │     │  ┌─────────────┐  │     │  ┌─────────────┐  │
│  │ ITextAnalysis│ │     │  │ TextAnalysis│  │     │  │ Semantic    │  │
│  │ Service     │◄─┼─────┼──┤ Service     │◄─┼─────┼──┤ Kernel      │  │
│  │             │  │     │  │             │  │     │  │             │  │
│  └─────────────┘  │     │  └─────────────┘  │     │  └─────────────┘  │
└───────────────────┘     └───────────────────┘     └───────────────────┘
```

## Project Structure

```
ReiStacks.ML.Integration/
├── Clients/
│   ├── AzureCognitiveServiceClient.cs
│   └── SemanticKernelClient.cs
├── Services/
│   ├── PropertyValuationService.cs
│   ├── LeadScoringService.cs
│   └── TextAnalysisService.cs
├── Configuration/
│   └── AzureAiServicesConfig.cs
├── Extensions/
│   └── ServiceCollectionExtensions.cs
└── ReiStacks.ML.Integration.csproj
```

## Key Components

### 1. Clients

#### AzureCognitiveServiceClient
Manages connections to Azure Cognitive Services:
- Form Recognizer for document analysis
- Text Analytics for sentiment and key phrase extraction
- Custom Vision for property image analysis

#### SemanticKernelClient
Provides access to Semantic Kernel capabilities:
- Semantic functions for natural language understanding
- Orchestration of AI planning
- Memory for context persistence

### 2. Services

#### PropertyValuationService
Implements the `IPropertyValuationService` interface from the Application layer:
- Predicts property values based on features
- Analyzes comparable properties
- Integrates with external data sources

#### LeadScoringService
Implements the `ILeadScoringService` interface:
- Scores leads based on historical data
- Predicts conversion probability
- Recommends follow-up strategies

#### TextAnalysisService
Implements the `ITextAnalysisService` interface:
- Extracts key information from property documents
- Analyzes lead communications
- Generates property descriptions

### 3. Configuration

#### AzureAiServicesConfig
Contains configuration settings for Azure services:
- API endpoints
- Authentication keys
- Service-specific settings

### 4. Extensions

#### ServiceCollectionExtensions
Provides extension methods for dependency injection:
```csharp
public static IServiceCollection AddMlIntegration(
    this IServiceCollection services, 
    IConfiguration configuration)
{
    // Register configurations
    services.Configure<AzureAiServicesConfig>(
        configuration.GetSection("AzureAi"));
    
    // Register clients
    services.AddSingleton<IAzureCognitiveServiceClient, AzureCognitiveServiceClient>();
    services.AddSingleton<ISemanticKernelClient, SemanticKernelClient>();
    
    // Register services
    services.AddScoped<IPropertyValuationService, PropertyValuationService>();
    services.AddScoped<ILeadScoringService, LeadScoringService>();
    services.AddScoped<ITextAnalysisService, TextAnalysisService>();
    
    return services;
}
```

## Implementation Guidelines

### Clean Architecture Compliance
- Services implement interfaces defined in the Application layer
- No direct dependencies from Application to ML.Integration
- All ML-specific dependencies isolated in this project

### Azure Integration

Configure Azure services in `appsettings.json`:
```json
{
  "AzureAi": {
    "OpenAi": {
      "Endpoint": "https://your-endpoint.openai.azure.com/",
      "ApiKey": "your-api-key",
      "Deployment": "your-deployment-name"
    },
    "CognitiveServices": {
      "Endpoint": "https://your-endpoint.cognitiveservices.azure.com/",
      "ApiKey": "your-api-key"
    }
  }
}
```

### Semantic Kernel Setup

Basic setup in `SemanticKernelClient.cs`:
```csharp
public class SemanticKernelClient : ISemanticKernelClient
{
    private readonly IKernel _kernel;
    
    public SemanticKernelClient(IOptions<AzureAiServicesConfig> options)
    {
        var builder = new KernelBuilder();
        
        builder.WithAzureChatCompletionService(
            options.Value.OpenAi.Deployment,
            options.Value.OpenAi.Endpoint,
            options.Value.OpenAi.ApiKey);
            
        _kernel = builder.Build();
        
        // Register plugins/skills
        RegisterPlugins();
    }
    
    // Implementation details...
}
```

## Usage Examples

### Property Valuation
```csharp
public class ParcelService
{
    private readonly IPropertyValuationService _valuationService;
    
    public ParcelService(IPropertyValuationService valuationService)
    {
        _valuationService = valuationService;
    }
    
    public async Task<decimal> EstimatePropertyValue(ParcelDto parcel)
    {
        var features = new PropertyFeatures
        {
            SquareFeet = parcel.SquareFeet,
            Bedrooms = parcel.Bedrooms,
            Bathrooms = parcel.Bathrooms,
            YearBuilt = parcel.YearBuilt,
            Latitude = parcel.Latitude,
            Longitude = parcel.Longitude,
            // Additional features
        };
        
        return await _valuationService.PredictPropertyValue(features);
    }
}
```

### Document Analysis
```csharp
public class LeadService
{
    private readonly ITextAnalysisService _textService;
    
    public LeadService(ITextAnalysisService textService)
    {
        _textService = textService;
    }
    
    public async Task<LeadInsights> AnalyzeLeadCommunication(string message)
    {
        return await _textService.AnalyzeText(message);
    }
}
```

## Testing

The ML Integration layer should be thoroughly tested:

1. Unit tests with mocked Azure services
2. Integration tests with actual Azure services (using test endpoints)
3. Performance tests to ensure acceptable response times

## Security Considerations

- Store API keys and secrets in Azure Key Vault
- Use Managed Identities where possible
- Implement proper error handling for failed AI service calls
- Validate and sanitize all inputs before sending to AI services

## Future Enhancements

- Add model retraining capabilities
- Implement A/B testing framework for AI models
- Add more advanced property features analysis
- Integrate with additional data sources