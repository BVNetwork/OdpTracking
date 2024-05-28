using System.Net;

namespace OdpTracking.Extensions
{
    public static class HttpStatusCodeExtensions
    {
        public static bool IsSuccessStatusCode(this HttpStatusCode statusCode)
        {
            var asInt = (int)statusCode;
            return asInt is >= 200 and <= 299;
        }
    }
}
