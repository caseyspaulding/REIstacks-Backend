

# Create a new migration
dotnet ef migrations add AddLeadListFilesTable --project ../REIstacks.Infrastructure

# Apply the migration to your database
dotnet ef database update --project REIstacks.Infrastructure --startup-project REIstacks.WebApi

# Remove the last migration
dotnet ef migrations remove --project ../REIstacks.Infrastructure