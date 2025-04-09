# ReiStacks - Real Estate Investment Technology Stack

ReiStacks is a comprehensive solution for real estate investors to analyze properties, manage leads, and score potential deals using AI and machine learning.

## Architecture Overview

This solution follows Clean Architecture principles to ensure:
- Separation of concerns
- Domain-centric design
- Testability
- Independence from frameworks and external services

### Project Structure

```
ReiStacks.sln
│
├── src/
│   ├── Core/
│   │   ├── ReiStacks.Domain/
│   │   │   ├── Entities/
│   │   │   │   ├── FloridaParcel.cs
│   │   │   │   ├── Lead.cs
│   │   │   │   └── Tenant.cs
│   │   │   ├── ValueObjects/
│   │   │   │   ├── DealScore.cs
│   │   │   │   └── Address.cs
│   │   │   ├── Enums/
│   │   │   │   ├── PropertyStatus.cs
│   │   │   │   └── LeadStatus.cs
│   │   │   └── ReiStacks.Domain.csproj
│   │   │
│   │   └── ReiStacks.Application/
│   │       ├── Interfaces/
│   │       │   ├── Repositories/
│   │       │   │   ├── IParcelRepository.cs
│   │       │   │   ├── ILeadRepository.cs
│   │       │   │   └── ITenantRepository.cs
│   │       │   ├── Services/
│   │       │   │   ├── IStorageService.cs
│   │       │   │   ├── IMlScoringService.cs
│   │       │   │   └── IRentCastService.cs
│   │       │   │   ├── IAiServices/
│   │       │   │   │   ├── IPropertyValuationService.cs
│   │       │   │   │   ├── ILeadScoringService.cs
│   │       │   │   │   └── ITextAnalysisService.cs
│   │       │   └── IUnitOfWork.cs
│   │       ├── Services/
│   │       │   ├── ParcelService.cs
│   │       │   ├── LeadService.cs
│   │       │   ├── TenantService.cs
│   │       │   └── DealScoringService.cs
│   │       ├── DTOs/
│   │       │   ├── ParcelDto.cs
│   │       │   ├── LeadDto.cs
│   │       │   └── TenantDto.cs
│   │       └── ReiStacks.Application.csproj
│   │
│   ├── Infrastructure/
│   │   ├── ReiStacks.Infrastructure/
│   │   │   ├── Data/
│   │   │   │   ├── ApplicationDbContext.cs
│   │   │   │   ├── EntityConfigurations/
│   │   │   │   │   ├── ParcelConfiguration.cs
│   │   │   │   │   ├── LeadConfiguration.cs
│   │   │   │   │   └── TenantConfiguration.cs
│   │   │   │   └── Migrations/
│   │   │   ├── Repositories/
│   │   │   │   ├── ParcelRepository.cs
│   │   │   │   ├── LeadRepository.cs
│   │   │   │   ├── TenantRepository.cs
│   │   │   │   └── UnitOfWork.cs
│   │   │   ├── Services/
│   │   │   │   ├── AzureBlobStorageService.cs
│   │   │   │   ├── MlScoringService.cs
│   │   │   │   └── RentCastService.cs
│   │   │   └── ReiStacks.Infrastructure.csproj
│   │   │
│   │   └── ReiStacks.ML.Integration/
│   │       ├── Clients/
│   │       │   ├── AzureCognitiveServiceClient.cs
│   │       │   └── SemanticKernelClient.cs
│   │       ├── Services/
│   │       │   ├── PropertyValuationService.cs
│   │       │   ├── LeadScoringService.cs
│   │       │   └── TextAnalysisService.cs
│   │       ├── Configuration/
│   │       │   └── AzureAiServicesConfig.cs
│   │       ├── Extensions/
│   │       │   └── ServiceCollectionExtensions.cs
│   │       └── ReiStacks.ML.Integration.csproj
│   │
│   ├── Presentation/
│   │   ├── ReiStacks.WebApi/
│   │   │   ├── Controllers/
│   │   │   │   ├── ParcelController.cs
│   │   │   │   ├── LeadController.cs
│   │   │   │   └── TenantController.cs
│   │   │   ├── Middleware/
│   │   │   │   ├── TenantMiddleware.cs
│   │   │   │   └── ExceptionHandlingMiddleware.cs
│   │   │   ├── Program.cs
│   │   │   ├── appsettings.json
│   │   │   └── ReiStacks.WebApi.csproj
│   │   │
│   │   └── ReiStacks.Functions/
│   │       ├── Functions/
│   │       │   ├── ProcessParcelDataFunction.cs
│   │       │   ├── ScoreDealsFunction.cs
│   │       │   └── GenerateLeadReportsFunction.cs
│   │       │   ├── AiAnalysisFunction.cs
│   │       ├── Startup.cs
│   │       ├── host.json
│   │       └── ReiStacks.Functions.csproj
│   │
├── tests/
│   ├── ReiStacks.Domain.Tests/
│   ├── ReiStacks.Application.Tests/
│   ├── ReiStacks.Infrastructure.Tests/
│   ├── ReiStacks.ML.Integration.Tests/
│   ├── ReiStacks.WebApi.Tests/
│   └── ReiStacks.Functions.Tests/
│
└── docs/
    ├── architecture/
    │   ├── clean-architecture.md
    │   └── ml-integration.md
    └── api/
        └── swagger.json
```

### Frontend

The frontend is maintained in a separate repository:
- ReiStacks.Web (Next.js application)

## Layer Responsibilities

### Core Layer

#### Domain Project
- Contains enterprise-wide business rules
- Entities (FloridaParcel, Lead, Tenant)
- Value objects (DealScore, Address)
- Enums and constants

#### Application Project
- Contains application-specific business rules
- Service interfaces and implementations
- DTOs for data transfer
- Repository interfaces

### Infrastructure Layer

#### Infrastructure Project
- Database access via Entity Framework Core
- Third-party service integrations
- Repository implementations
- Unit of work pattern implementation

#### ML.Integration Project
- Azure Cognitive Services integration
- Semantic Kernel for AI capabilities
- Property valuation models
- Lead scoring services

### Presentation Layer

#### WebApi Project
- RESTful API endpoints
- Authentication and authorization
- Swagger documentation
- Middleware components

#### Functions Project
- Serverless Azure Functions
- Background processing
- Scheduled tasks
- Event-driven workflows

## Dependency Flow

Dependencies always point inward, following Clean Architecture principles:
- Domain has no dependencies
- Application depends on Domain
- Infrastructure depends on Application and Domain
- Presentation depends on Application, Infrastructure, and Domain

## Getting Started

1. Clone the repository
2. Open the solution in Visual Studio
3. Set startup project to ReiStacks.WebApi or ReiStacks.Functions
4. Configure connection strings in appsettings.json
5. Run migrations to create the database
6. Start debugging

## Key Features

- Property analysis and valuation
- Lead tracking and management
- AI-powered deal scoring
- Tenant management
- Integration with external real estate data sources

## AI and ML Integration

The ML.Integration project leverages:
- Azure Cognitive Services for document analysis
- Semantic Kernel for natural language understanding
- Custom ML models for property valuation and lead scoring

## Testing Strategy

Each project has a corresponding test project for:
- Unit tests
- Integration tests
- End-to-end tests

## Contributing

Please follow these guidelines:
- Maintain separation of concerns per Clean Architecture
- Write tests for all new functionality
- Follow C# coding conventions
- Use feature branches and pull requests

## License

[Specify license information]
