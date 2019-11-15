using System.Threading;
using System.Threading.Tasks;
using DfE.EmployerFavourites.Web.Infrastructure.FavouritesApiClient;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace DfE.EmployerFavourites.Infrastructure.HealthChecks
{
    public class FavouritesApiHealthCheck : IHealthCheck
    {
        private readonly IFavouritesApi _repository;

        public FavouritesApiHealthCheck(IFavouritesApi favouritesRepository)
        {
            _repository = favouritesRepository;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(
            HealthCheckContext context,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var healthCheckResultHealthy = true;

            try
            {
                await _repository.GetHealthAsync();
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