// REIstacks.Application.Interfaces.IServices/ISkipTraceService.cs

using REIstacks.Application.Contracts.Requests;

namespace REIstacks.Application.Interfaces.IServices
{
    public interface ISkipTraceService
    {
        /// <summary>
        /// Start skip‐trace by existing contact IDs (looks up addresses internally).
        /// </summary>
        Task<SkipTraceActivity> StartSkipTraceAsync(
            IEnumerable<int> contactIds,
            string organizationId
        );

        /// <summary>
        /// Start skip‐trace over arbitrary addresses (CSV upload case).
        /// </summary>
        Task<SkipTraceActivity> StartSkipTraceByAddressesAsync(
            IEnumerable<AddressCsvDto> addresses,
            string organizationId
        );

        /// <summary>
        /// Translate contact IDs → AddressCsvDto rows.
        /// </summary>
        Task<IEnumerable<AddressCsvDto>> GetAddressesByContactIdsAsync(
            IEnumerable<int> contactIds,
            string organizationId
        );

        /// <summary>
        /// List all skip‐trace runs for this tenant.
        /// </summary>
        Task<IEnumerable<SkipTraceActivity>> GetActivitiesAsync(
            string organizationId
        );

        /// <summary>
        /// Fetch the details of a single run (with breakdown & items).
        /// </summary>
        Task<SkipTraceActivity?> GetActivityByIdAsync(
            int id,
            string organizationId
        );

        /// <summary>
        /// Cancel an in‐progress run.
        /// </summary>
        Task<bool> CancelActivityAsync(
            int id,
            string organizationId
        );
    }
}
