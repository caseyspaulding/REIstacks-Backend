using Microsoft.Extensions.Logging;
using REIstacks.Application.Interfaces;
using REIstacks.Application.Repositories.Interfaces;
using REIstacks.Domain.Entities.Organizations;

namespace REIstacks.Infrastructure.Services.Organizations
{
    public class DomainVerificationService : IDomainVerificationService
    {
        private readonly IDomainVerificationRepository _domainRepository;
        private readonly IOrganizationRepository _organizationRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<DomainVerificationService> _logger;

        public DomainVerificationService(
            IDomainVerificationRepository domainRepository,
            IOrganizationRepository organizationRepository,
            IUnitOfWork unitOfWork,
            ILogger<DomainVerificationService> logger)
        {
            _domainRepository = domainRepository;
            _organizationRepository = organizationRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<IEnumerable<DomainVerification>> GetPendingVerificationsAsync(string organizationId)
        {
            if (string.IsNullOrWhiteSpace(organizationId))
                throw new ArgumentException("Invalid organization ID", nameof(organizationId));

            var allVerifications = await _domainRepository.GetByOrganizationIdAsync(organizationId);
            var pendingVerifications = allVerifications.Where(dv => !dv.IsVerified);

            _logger.LogInformation("Retrieved {Count} pending verifications for organization {OrganizationId}",
                pendingVerifications.Count(), organizationId);

            return pendingVerifications;
        }


        public async Task<string> GenerateVerificationTokenAsync(string organizationId, string domain)
        {
            if (string.IsNullOrWhiteSpace(organizationId) || string.IsNullOrWhiteSpace(domain))
                throw new ArgumentException("Invalid parameters");

            domain = NormalizeDomain(domain);

            var existingVerification = await _domainRepository.GetByDomainAsync(domain);

            if (existingVerification != null && existingVerification.IsVerified)
            {
                throw new InvalidOperationException($"Domain {domain} is already verified");
            }

            string token = GenerateRandomToken();

            if (existingVerification != null)
            {
                existingVerification.VerificationToken = token;
                // Update other properties as needed
            }
            else
            {
                var newVerification = new DomainVerification
                {
                    OrganizationId = organizationId,
                    Domain = domain,
                    VerificationToken = token,
                    IsVerified = false,
                    CreatedAt = DateTime.UtcNow
                };
                await _domainRepository.AddAsync(newVerification);
            }

            await _unitOfWork.CompleteAsync();
            return token;
        }

        // Other methods refactored similarly...

        private string NormalizeDomain(string domain)
        {
            // Your existing implementation
            return domain;
        }

        private string GenerateRandomToken()
        {
            // Your existing implementation
            return string.Empty;
        }

        public async Task<bool> VerifyDomainAsync(string organizationId, string domain, string token)
        {
            if (string.IsNullOrWhiteSpace(organizationId) || string.IsNullOrWhiteSpace(domain) || string.IsNullOrWhiteSpace(token))
                throw new ArgumentException("Invalid parameters");

            domain = NormalizeDomain(domain);

            var verification = await _domainRepository.GetByDomainAsync(domain);
            if (verification == null || verification.OrganizationId != organizationId)
            {
                _logger.LogWarning("Verification attempt for domain {Domain} failed: Domain not found or does not belong to organization {OrganizationId}", domain, organizationId);
                return false;
            }

            if (verification.IsVerified)
            {
                _logger.LogInformation("Domain {Domain} is already verified for organization {OrganizationId}", domain, organizationId);
                return true;
            }

            if (verification.VerificationToken != token)
            {
                _logger.LogWarning("Verification attempt for domain {Domain} failed: Invalid token", domain);
                return false;
            }

            var result = await _domainRepository.MarkAsVerifiedAsync(domain, organizationId);
            if (result)
            {
                await _unitOfWork.CompleteAsync();
                _logger.LogInformation("Domain {Domain} successfully verified for organization {OrganizationId}", domain, organizationId);
            }

            return result;
        }
    }
}