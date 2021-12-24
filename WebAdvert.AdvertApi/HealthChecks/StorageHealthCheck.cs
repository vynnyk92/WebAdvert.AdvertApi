using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using WebAdvert.AdvertApi.Services;

namespace WebAdvert.AdvertApi.HealthChecks
{
    public class StorageHealthCheck : IHealthCheck
    {
        private IAdvertStorageService _advertStorageService;

        public StorageHealthCheck(IAdvertStorageService advertStorageService)
        {
            _advertStorageService = advertStorageService;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            return await _advertStorageService.HealthCheckAsync()
                ? HealthCheckResult.Healthy()
                : HealthCheckResult.Unhealthy();
        }
    }
}
