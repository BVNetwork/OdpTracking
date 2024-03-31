using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace OdpTracking.Client
{
    public class ConfigurationTrackerIdProvider : ITrackerIdProvider
    {
        private const string OdpTrackingIdConfigKey = "OdpTrackingId";
        private static string _odpTrackingId = "";

        public ConfigurationTrackerIdProvider(IConfiguration configuration)
        {
            string trackerId = configuration[OdpTrackingIdConfigKey];
            if (string.IsNullOrEmpty(trackerId) == false)
            {
                _odpTrackingId = trackerId;
            }
            else
            {
                _odpTrackingId = string.Empty;
            }
        }
        
        /// <summary>
        /// Gets the configured tracking id if any
        /// </summary>
        /// <returns>An ODP tracking id, or string.Empty if not configured.</returns>
        public string GetTrackerId()
        {
            return _odpTrackingId;
        }

        /// <summary>
        /// Since this provider gets the tracking id from the config, it cannot be set
        /// per request. Use the <see cref="HttpContextOdpTrackerIdProvider"/> if you
        /// need to change the tracking id for a request
        /// </summary>
        /// <param name="httpContext"></param>
        /// <param name="trackerId"></param>
        /// <exception cref="NotImplementedException"></exception>
        public void SetTrackerIdForRequest(HttpContext httpContext, string trackerId)
        {
            throw new NotImplementedException("The tracker ID cannot be set when using this provider.");
        }
    }
}