using Microsoft.Extensions.DependencyInjection;
// ReSharper disable UnusedType.Global

namespace OdpTracking
{
    public static class OdpTrackingServiceExtensions
    {
        public static IServiceCollection AddOdpTracking(this IServiceCollection services)
        {
            services.AddTransient<IOdpServerSideTracker, OdpServerSideTracker>();
            services.AddTransient<IOdpTrackerOptions, OdpTrackerOptions>();
            services.AddTransient<IOdpHttpClientHelper, OdpHttpClientHelper>();
            
            return services;
        }
    }
}

