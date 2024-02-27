using System.Net;

namespace OdpTracking
{
    public interface IOdpHttpClientHelper
    {
        public T? GetJson<T>(string apiMethod, string query, string payLoad);
        public HttpStatusCode PostJson(string apiMethod, string payLoad);
    }
}