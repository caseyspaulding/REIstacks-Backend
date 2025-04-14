
# 🧱 Process for Adding a New Controller in REIstacks

A step-by-step guide for adding a new feature/controller to the REIstacks application based on the `Contact` implementation example.

---

## 1. Define the Entity

Create a C# class in the Domain layer representing your database table:

```csharp
// REIstacks.Domain/Entities/[YourDomain]/YourEntity.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace REIstacks.Domain.Entities.[YourDomain];

[Table("your_entities")]
public class YourEntity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    public string OrganizationId { get; set; }

    // Add your entity properties here

    // Navigation properties
    [ForeignKey("OrganizationId")]
    public virtual Organization Organization { get; set; }
}
```

---

## 2. Add `DbSet` to `AppDbContext`

```csharp
// REIstacks.Infrastructure/Data/AppDbContext.cs
public DbSet<YourEntity> YourEntities { get; set; }
```

---

## 3. Create Repository Interface

```csharp
// REIstacks.Application/Repositories/Interfaces/IYourEntityRepository.cs
public interface IYourEntityRepository
{
    Task<YourEntity> GetByIdAsync(int id);
    Task<IEnumerable<YourEntity>> GetAllAsync();
    Task<(IEnumerable<YourEntity> Entities, int TotalCount)> GetPagedAsync(int page, int pageSize, string organizationId);
    Task<IEnumerable<YourEntity>> GetByOrganizationIdAsync(string organizationId);
    Task<int> AddAsync(YourEntity entity);
    void Update(YourEntity entity);
    void Delete(YourEntity entity);
    Task<bool> ExistsAsync(int id);
}
```

---

## 4. Implement Repository

```csharp
// REIstacks.Infrastructure/Repositories/[YourDomain]/YourEntityRepository.cs
public class YourEntityRepository : Repository<YourEntity>, IYourEntityRepository
{
    private readonly AppDbContext _context;

    public YourEntityRepository(AppDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<(IEnumerable<YourEntity> Entities, int TotalCount)> GetPagedAsync(int page, int pageSize, string organizationId)
    {
        var query = _context.YourEntities
            .Where(e => e.OrganizationId == organizationId)
            .OrderByDescending(e => e.CreatedAt);

        var totalCount = await query.CountAsync();
        var entities = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Include(e => e.Organization)
            .ToListAsync();

        return (entities, totalCount);
    }

    public async Task<IEnumerable<YourEntity>> GetByOrganizationIdAsync(string organizationId)
    {
        return await _context.YourEntities
            .Where(e => e.OrganizationId == organizationId)
            .OrderByDescending(e => e.CreatedAt)
            .ToListAsync();
    }
}
```

---

## 5. Create Service Interface

```csharp
// REIstacks.Application/Services/Interfaces/IYourEntityService.cs
public interface IYourEntityService
{
    Task<YourEntity> GetEntityByIdAsync(int id);
    Task<(IEnumerable<YourEntity> Entities, int TotalCount, int TotalPages)> GetPagedEntitiesAsync(int page, int pageSize, string organizationId);
    Task<IEnumerable<YourEntity>> GetEntitiesByOrganizationAsync(string organizationId);
    Task<int> CreateEntityAsync(YourEntity entity);
    Task UpdateEntityAsync(YourEntity entity);
    Task DeleteEntityAsync(int id);
}
```

---

## 6. Implement Service

```csharp
// REIstacks.Infrastructure/Services/[YourDomain]/YourEntityService.cs
public class YourEntityService : IYourEntityService
{
    private readonly IUnitOfWork _unitOfWork;

    public YourEntityService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<YourEntity> GetEntityByIdAsync(int id)
        => await _unitOfWork.YourEntities.GetByIdAsync(id);

    public async Task<(IEnumerable<YourEntity> Entities, int TotalCount, int TotalPages)> GetPagedEntitiesAsync(int page, int pageSize, string organizationId)
    {
        var (entities, totalCount) = await _unitOfWork.YourEntities.GetPagedAsync(page, pageSize, organizationId);
        int totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
        return (entities, totalCount, totalPages);
    }

    public async Task<IEnumerable<YourEntity>> GetEntitiesByOrganizationAsync(string organizationId)
        => await _unitOfWork.YourEntities.GetByOrganizationIdAsync(organizationId);

    public async Task<int> CreateEntityAsync(YourEntity entity)
    {
        entity.CreatedAt = DateTime.UtcNow;
        entity.UpdatedAt = DateTime.UtcNow;
        await _unitOfWork.YourEntities.AddAsync(entity);
        await _unitOfWork.CompleteAsync();
        return entity.Id;
    }

    public async Task UpdateEntityAsync(YourEntity entity)
    {
        var existing = await _unitOfWork.YourEntities.GetByIdAsync(entity.Id);
        if (existing == null) throw new KeyNotFoundException();

        entity.CreatedAt = existing.CreatedAt;
        entity.UpdatedAt = DateTime.UtcNow;

        _unitOfWork.YourEntities.Update(entity);
        await _unitOfWork.CompleteAsync();
    }

    public async Task DeleteEntityAsync(int id)
    {
        var entity = await _unitOfWork.YourEntities.GetByIdAsync(id);
        if (entity == null) throw new KeyNotFoundException();

        _unitOfWork.YourEntities.Delete(entity);
        await _unitOfWork.CompleteAsync();
    }
}
```

---

## 7. Update `IUnitOfWork` Interface

```csharp
public interface IUnitOfWork : IDisposable
{
    IYourEntityRepository YourEntities { get; }
    Task<int> CompleteAsync();
}
```

---

## 8. Update `UnitOfWork` Implementation

```csharp
public class UnitOfWork : IUnitOfWork
{
    public IYourEntityRepository YourEntities { get; }

    public UnitOfWork(IYourEntityRepository yourEntities)
    {
        YourEntities = yourEntities;
    }

    public Task<int> CompleteAsync() => _context.SaveChangesAsync();
}
```

---

## 9. Register in DI Container

```csharp
public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
{
    services.AddScoped<IYourEntityRepository, YourEntityRepository>();
    services.AddScoped<IYourEntityService, YourEntityService>();
    return services;
}
```

---

## 10. Create API Controller

Use the full controller implementation from your backend, or let me know if you’d like it embedded here.

---

## 11. Create Frontend Model and Service

Use your `models/your-entity.ts` and `services/YourEntity.service.ts` files with Axios or your preferred HTTP client.

---

## 12. Create Frontend Page Component

Use your `page.tsx` component implementation with PrimeReact data tables.

---

## 13. Run EF Core Migration

```bash
dotnet ef migrations add AddYourEntityTable --project REIstacks.Infrastructure --startup-project REIstacks.Api --output-dir Migrations
dotnet ef database update --project REIstacks.Infrastructure --startup-project REIstacks.Api
```

---

## ✅ Summary of Steps

- [x] Create Entity
- [x] Add `DbSet`
- [x] Create Repository Interface
- [x] Implement Repository
- [x] Create Service Interface
- [x] Implement Service
- [x] Update `UnitOfWork`
- [x] Register in DI Container
- [x] Create API Controller
- [x] Create Frontend Model + Service
- [x] Create Frontend Page
- [x] Run EF Migration

---

## 🧠 Why This Matters

This architecture ensures:

- Clean separation of concerns  
- Code reusability through repositories and services  
- Type safety through interfaces  
- Organization-based data isolation  
- Proper authentication and authorization
