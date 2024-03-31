using Microsoft.AspNetCore.Http;

namespace OdpTracking.Client
{
    /// <summary>
    /// Pluggable tracker ID provider. If you want to store the tracker ID
    /// somewhere else than the configuration file, implement and override this
    /// in your project.
    /// </summary>
    public interface ITrackerIdProvider
    {
        /// <summary>
        /// Gets the current tracker id for ODP. Can be null if no tracking has
        /// been configured.
        /// </summary>
        /// <returns></returns>
        public string GetTrackerId();
        /// <summary>
        /// Sets the tracker id for a request. Use this to override or set the tracker
        /// id per request. The implementation can then use that value.
        /// </summary>
        /// <param name="httpContext"></param>
        /// <param name="trackerId"></param>
        public void SetTrackerIdForRequest(HttpContext httpContext, string trackerId);
    }
}