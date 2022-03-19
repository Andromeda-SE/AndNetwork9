using Microsoft.Extensions.Diagnostics.HealthChecks;
using VkNet;

namespace And9.Integration.VK.HealthChecks;

public class VkHealthCheck : IHealthCheck
{
    private readonly VkApi _vkApi;
    public VkHealthCheck(VkApi vkApi)
    {
        _vkApi = vkApi;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            Task result = _vkApi.Utils.GetServerTimeAsync();
            await result.ConfigureAwait(false);
            return result.IsCompletedSuccessfully ? HealthCheckResult.Healthy() : HealthCheckResult.Unhealthy(result.Exception?.Message);
        }
        catch
        {
            return HealthCheckResult.Unhealthy();
        }
    }
}