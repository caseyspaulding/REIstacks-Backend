

using REIstacks.Application.Repositories.Interfaces;

namespace REIstacks.Infrastructure.Services;

public abstract class BaseService
{
    protected readonly IUnitOfWork _unitOfWork;

    public BaseService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }

    // Common validation method
    protected void ValidateNotNull<T>(T entity, string entityName) where T : class
    {
        if (entity == null)
            throw new ArgumentNullException($"{entityName} cannot be null");
    }

    // Common error handling method
    protected T ExecuteWithErrorHandling<T>(Func<T> action, string errorMessage)
    {
        try
        {
            return action();
        }
        catch (Exception ex)
        {
            // Log exception
            throw new Exception($"{errorMessage}: {ex.Message}", ex);
        }
    }
}