using System.Threading;
using System.Threading.Tasks;
using DfE.EmployerFavourites.Web.Infrastructure.FatApiClient;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace DfE.EmployerFavourites.Infrastructure.HealthChecks
{
    public class FatApiHealthCheck : IHealthCheck
    {
        private readonly IFatApi _repository;

        public FatApiHealthCheck(IFatApi fatRepository)
        {
            _repository = fatRepository;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(
            HealthCheckContext context,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var healthCheckResultHealthy = true;

            try
            {
                
                var result = await _repository.GetStandardAsync("50"); // Chartered surveyor
            }
            catch
            {
                healthCheckResultHealthy = false;
            }

            if (healthCheckResultHealthy)
            {
                return HealthCheckResult.Healthy();
            }

            return HealthCheckResult.Unhealthy();
        }
    }
}