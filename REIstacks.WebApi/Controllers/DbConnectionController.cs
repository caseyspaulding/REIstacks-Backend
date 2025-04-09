using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using REIstacks.Infrastructure.Data;


namespace reistacks_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DbConnectionController : ControllerBase
    {
        private readonly AppDbContext _dbContext;

        public DbConnectionController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        public async Task<IActionResult> TestConnection()
        {
            try
            {
                // Try to connect to the database
                bool canConnect = await _dbContext.Database.CanConnectAsync();

                if (canConnect)
                {
                    return Ok(new
                    {
                        Status = "Success",
                        Message = "Successfully connected to the database"
                    });
                }
                else
                {
                    return StatusCode(500, new
                    {
                        Status = "Error",
                        Message = "Could not connect to the database"
                    });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Status = "Error",
                    Message = $"Exception occurred: {ex.Message}",
                    Detail = ex.InnerException?.Message
                });
            }
        }
        [HttpGet("migrations")]
        public async Task<IActionResult> TestMigrations()
        {
            try
            {
                // Check if there are any pending migrations
                var pendingMigrations = await _dbContext.Database.GetPendingMigrationsAsync();
                var appliedMigrations = await _dbContext.Database.GetAppliedMigrationsAsync();

                return Ok(new
                {
                    Status = "Success",
                    PendingMigrations = pendingMigrations.ToList(),
                    AppliedMigrations = appliedMigrations.ToList()
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Status = "Error",
                    Message = $"Exception occurred: {ex.Message}",
                    Detail = ex.InnerException?.Message
                });
            }
        }
    }

}
