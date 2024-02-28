using Microsoft.AspNetCore.Http;

namespace OdpTracking
{
    /// <summary>
    /// Pluggable tracker ID provider. If you want to store the tracker ID
    /// somewhere else than the configuration file, implement and override this
    /// in your project.
    /// </summary>
    public interface ITrackerIdProvider
    {
        public string GetTrackerId();
        public void SetTrackerIdItem(HttpContext httpContext, string trackerId);
    }
}