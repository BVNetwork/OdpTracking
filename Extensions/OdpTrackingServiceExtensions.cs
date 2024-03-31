using Microsoft.Extensions.DependencyInjection;
using OdpTracking.Client;
using OdpTracking.Configuration;
using OdpTracking.Http;

// ReSharper disable UnusedType.Global

namespace OdpTracking.Extensions
{
    public static class OdpTrackingServiceExtensions
    {
        public static IServiceCollection AddOdpTracking(this IServiceCollection services)
        {
            services.AddTransient<IOdpServerSideTracker, OdpServerSideTracker>();
            services.AddTransient<IOdpTrackerOptions, OdpTrackerOptions>();
            services.AddTransient<IOdpHttpClientHelper, OdpHttpClientHelper>();
            services.AddTransient<ITrackerIdProvider, ConfigurationTrackerIdProvider>();
            
            return services;
        }
    }
}

