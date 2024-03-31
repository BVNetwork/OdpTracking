using Microsoft.Extensions.Configuration;
// ReSharper disable InconsistentNaming
// ReSharper disable IdentifierTypo

namespace OdpTracking.Configuration
{
    public class OdpTrackerOptions : IOdpTrackerOptions
    {
        
        private const string ODPPRIVATEKEY = "OdpPrivateKey";
        private const string ODPAPIBASEURL = "OdpApiBaseUrl";

        public OdpTrackerOptions(IConfiguration configuration)
        {
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

    }
}