using System.Threading;
using System.Threading.Tasks;
using DfE.EmployerFavourites.Api.Infrastructure.Interfaces;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace DfE.EmployerFavourites.Api.Infrastructure.HealthChecks
{
    public class FatApiHealthCheck : IHealthCheck
    {
        private readonly IFatRepository _repository;

        public FatApiHealthCheck(IFatRepository fatRepository)
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
                var result = await _repository.GetApprenticeshipNameAsync("50"); // Chartered surveyor
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