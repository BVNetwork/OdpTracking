using Microsoft.AspNetCore.Http;

namespace OdpTracking.Middleware
{
    /// <summary>
    /// When someone clicks a link in a newsletter with a query string we recognize,
    /// we will log this as a customer event in ODP. If the email is opened in a browser
    /// we haven't seen before, or the user has not identified themselves in this browser
    /// the system will stitch these two profiles together.
    /// </summary>
    /// <remarks>
    /// If someone forwards a newsletter, we risk stitching the wrong profile. We can avoid 
    /// this if we check if the current vuid is connected to a known profile first. If the 
    /// email has been forwarded but belongs to an actual new user, their combined browsing
    /// will be tracked under the same profile.
    /// If the new user should identify themselves with a new email later, the connection
    /// will be untangled, and the vuid will be removed from the old profile and used for
    /// another profile.
    /// </remarks>
    public class OdpEmailStitcherMiddleware
    {

        private readonly RequestDelegate _next;
        private readonly IOdpServerSideTracker _odpServerSideTracker;

        public OdpEmailStitcherMiddleware(RequestDelegate next, IOdpServerSideTracker odpServerSideTracker)
        {
            _next = next;
            _odpServerSideTracker = odpServerSideTracker;
        }

        public async Task Invoke(HttpContext context)
        {
            if (context.Request.Method.Equals("get", System.StringComparison.InvariantCultureIgnoreCase) &&
                context.Request.Query.ContainsKey("recipientEmail"))
            {
                // Note! We do not want to do any lookup or verification here. If we want to add more logic
                // to the stitching we should do that in a background queue.
                Microsoft.Extensions.Primitives.StringValues email;
                if (context.Request.Query.TryGetValue("recipientEmail", out email))
                {
                    if (email.Count > 0)
                    {
                        string vuid = context.Request.GetVuid();
                        if (vuid != null)
                        {
                            _odpServerSideTracker.UpdateProfile(vuid, email[0]);
                        }
                    }
                }
            }

            await _next(context);
        }
    }
}