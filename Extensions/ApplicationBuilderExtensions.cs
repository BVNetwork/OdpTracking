using Microsoft.AspNetCore.Builder;
using OdpTracking.Middleware;

namespace OdpTracking.Extensions
{
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseOdpTracking(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<OdpEmailStitcherMiddleware>();
        }
    

    }
}