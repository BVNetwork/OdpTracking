using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace OdpTracking
{
    public class ConfigurationTrackerIdProvider : ITrackerIdProvider
    {
        private readonly IConfiguration _configuration;
        private const string OdpTrackingIdConfigKey = "OdpTrackingId";

        public ConfigurationTrackerIdProvider(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public string GetTrackerId()
        {
            string trackerId = _configuration[OdpTrackingIdConfigKey];
            if (string.IsNullOrEmpty(trackerId) == false)
            {
                return trackerId;
            }
            else
            {
                return string.Empty;
            }
        }

        public void SetTrackerIdItem(HttpContext httpContext, string trackerId)
        {
            throw new NotImplementedException("The tracker ID cannot be set when using this provider.");
        }
    }
}