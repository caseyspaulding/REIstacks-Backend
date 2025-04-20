// REIstacks.Api/Controllers/TenantController.cs
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace REIstacks.Api.Controllers
{
    [ApiController]
    public abstract class TenantController : ControllerBase
    {
        /// <summary>
        /// Pulls the organization_id claim or throws UnauthorizedAccessException.
        /// </summary>
        protected string OrgId
        {
            get
            {
                var org = User.FindFirstValue("organization_id");
                if (string.IsNullOrEmpty(org))
                    throw new UnauthorizedAccessException("Organization ID not found in user claims");
                return org;
            }
        }
    }
}
