using Microsoft.AspNetCore.Http;

namespace OdpTracking.Extensions
{
    public static class VuidExtensions
    {
        public static string GetVuid(this HttpRequest request)
        {
            return request.Cookies.GetVuid();
        }
        public static string GetVuid(this IRequestCookieCollection cookies)
        {
            if (cookies.TryGetValue("vuid", out var vuid))
            {
                return GetVuidFromCookieValue(vuid!); // Could still be null
            }
            return null!;
        }

        public static string GetVuidFromCookieValue(string cookieValue)
        {
            if (!string.IsNullOrWhiteSpace(cookieValue) && cookieValue.Length > 35)
            {
                return cookieValue.Substring(0, 36).Replace("-", string.Empty);
            }
            return null!;
        }

    }
}