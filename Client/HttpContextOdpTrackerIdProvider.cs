#nullable enable
using Microsoft.AspNetCore.Http;

namespace OdpTracking.Client
{
    /// <summary>
    /// Dynamic tracker id provider that picks the tracking id up from the HttpContext.
    /// Use this if you have different tracker ids for different sites in your solution. Set the
    /// tracker id as a property on your start page, and read it from there per request.
    /// </summary>
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

        public void SetTrackerIdForRequest(HttpContext httpContext, string trackerId)
        {
            if (_httpContextAccessor.HttpContext != null)
            {
                // Tracker ID can still be null
                _httpContextAccessor.HttpContext.Items[OdpTrackerIdKey] = trackerId;
            }
        }
    }
}