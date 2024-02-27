using Microsoft.Extensions.Configuration;

namespace OdpTracking
{
    public class OdpTrackerOptions : IOdpTrackerOptions
    {
        private readonly IConfiguration _configuration;
        private const string ODPPRIVATEKEY = "OdpPrivateKey";
        private const string ODPAPIBASEURL = "OdpApiBaseUrl";

        public OdpTrackerOptions(IConfiguration configuration)
        {
            _configuration = configuration;
            PrivateKey = configuration[ODPPRIVATEKEY];
            BaseUrl = configuration[ODPAPIBASEURL];
            if (string.IsNullOrEmpty(BaseUrl) == false)
            {
                BaseUrl = BaseUrl.TrimEnd('/');
            }
        }

        public string PrivateKey { get; }
        public string BaseUrl { get; }

        public bool IsConfigured()
        {
            if (string.IsNullOrEmpty(BaseUrl) ||
                string.IsNullOrEmpty(PrivateKey))
            {
                return false;
            }

            return true;
        }

        public Uri CreateUri(string path, string? query = null)
        {
            if(string.IsNullOrEmpty(path)) throw new ArgumentNullException(nameof(path), "Path cannot be null");
            
            string url = BaseUrl;
            if (path.StartsWith('/') == false)
            {
                url = url + '/' + path;
            }
            else
            {
                url = url + path;
            }

            if (string.IsNullOrEmpty(query) == false)
            {
                if (query.StartsWith('?') == false)
                {
                    url = url + '?' + query;
                }
                else
                {
                    url = url + query;
                }
            }

            return new Uri(url);
        }
    }
}