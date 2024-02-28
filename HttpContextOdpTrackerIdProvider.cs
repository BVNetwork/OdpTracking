#nullable enable
using Microsoft.AspNetCore.Http;

namespace OdpTracking
{
    public class HttpContextOdpTrackerIdProvider : ITrackerIdProvider
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private const string OdpTrackerIdKey = "OdpTrackerId";

        public HttpContextOdpTrackerIdProvider(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }
        
        public string GetTrackerId()
        {
            if (_httpContextAccessor.HttpContext != null)
            {
                if (_httpContextAccessor.HttpContext.Items.TryGetValue(OdpTrackerIdKey, out object? value))
                {
                    return (value != null ? value.ToString() : string.Empty)!;
                }
            }

            return string.Empty;
        }

        public void SetTrackerIdItem(HttpContext httpContext, string trackerId)
        {
            if (_httpContextAccessor.HttpContext != null)
            {
                // Tracker ID can still be null
                _httpContextAccessor.HttpContext.Items[OdpTrackerIdKey] = trackerId;
            }
        }
    }
}