using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Configuration;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using OdpTracking.Dto;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace OdpTracking
{

    public class OdpServerSideTracker : IOdpServerSideTracker
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<OdpServerSideTracker> _logger;
        private readonly IOdpHttpClientHelper _httpClientHelper;
        private readonly IOdpTrackerOptions _odpTrackerOptions;

        public OdpServerSideTracker(IHttpClientFactory httpClientFactory, 
            IConfiguration configuration, 
            ILogger<OdpServerSideTracker> logger,
            IOdpHttpClientHelper httpClientHelper,
            IOdpTrackerOptions odpTrackerOptions)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
            _httpClientHelper = httpClientHelper;
            _odpTrackerOptions = odpTrackerOptions;
        }

        public void TrackLogin(string email, string vuid)
        {
            if(_odpTrackerOptions.IsConfigured() == false)
                return;
            
            if (string.IsNullOrEmpty(email) == false && string.IsNullOrEmpty(vuid) == false)
            {
                /*
{
    "type": "account",
    "action": "login",
    "identifiers": {
        "email": "test_vuid_withunderscore_3@optimizely.com",
        "vuid": "6067f061d0f04b76812dc0308270a6e2"
    }
}                 
                 */

                var data = new OdpDtoEvent
                {
                    Action = "login",
                    Type = "account",
                    Identifiers = new OdpDtoIdentifiers
                    {
                        Email = email,
                        Vuid = vuid
                    }
                };
                string payLoad = SerializeToJson(data);

                PostPayload(payLoad, "/v3/events");
            }

        }

        public void TrackOrder(string email, string vuid, OdpDtoOrder order)
        {
            if(_odpTrackerOptions.IsConfigured() == false)
                return;

            if (string.IsNullOrEmpty(email) == false || string.IsNullOrEmpty(vuid) == false)
            {
                /*
{
    "type": "account",
    "action": "login",
    "identifiers": {
        "email": "test_vuid_withunderscore_3@optimizely.com",
        "vuid": "6067f061d0f04b76812dc0308270a6e2"
    }
}                 
                 */

                var data = new OdpDtoOrderEvent
                {
                    Action = "purchase",
                    Type = "order",
                    Identifiers = new OdpDtoIdentifiers
                    {
                        Email = email,
                        Vuid = vuid
                    },
                    Data = new { Order = order }
                };
                string payLoad = SerializeToJson(data);

                PostPayload(payLoad, "/v3/events");
            }
        }

        public void TrackEvent(OdpDtoEvent odpEvent)
        {
            if(_odpTrackerOptions.IsConfigured() == false)
                return;

            if (string.IsNullOrEmpty(odpEvent.Identifiers.Vuid) == false)
            {
                string payLoad = SerializeToJson(odpEvent);
                PostPayload(payLoad, "/v3/events");
            }
        }


        /// <summary>
        /// Connects Vuid and Email. Useful when you do not have
        /// any other information about the profile, or you want
        /// to connect a new vuid to an email.
        /// </summary>
        /// <param name="email"></param>
        /// <param name="vuid"></param>
        public void DiscoverProfile(string email, string vuid)
        {
            if(_odpTrackerOptions.IsConfigured() == false)
                return;

            if (string.IsNullOrEmpty(email) == false &&
                string.IsNullOrEmpty(vuid) == false)
            {
                var payLoad = SerializeToJson(
                    new
                    {
                        Attributes = new OdpDtoDiscoveryProfile
                        {
                            Email = EncodeEmail(email),
                            Vuid = vuid
                        }
                    });

                PostPayload(payLoad, "/v3/profiles");
            }
        }

        private string EncodeEmail(string email)
        {
            // Product Recs decodes the email address in the query string
            // and replaces "+" with " ". Let's add that back
            return email.Replace(" ", "+");
        }

        public void UpdateProfile(OdpDtoProfile profile)
        {
            if(_odpTrackerOptions.IsConfigured() == false)
                return;

            if (string.IsNullOrEmpty(profile.Email) == false &&
                string.IsNullOrEmpty(profile.Vuid) == false)
            {
                var payLoad = SerializeToJson(new { Attributes = profile });

                // {
                //     attributes:
                //     {
                //         email: " + email + @",
                //         vuid: " + vuid + @" 
                //     }
                // }

                PostPayload(payLoad, "/v3/profiles");
            }
        }

        public void UploadProducts(IEnumerable<OdpDtoProduct> products)
        {
            var payLoad = SerializeToJson(products);
            PostPayload(payLoad, "/v3/objects/products");
        }

        private void PostPayload(string payLoad, string apiMethod)
        {
            var statusCode = _httpClientHelper.PostJson(apiMethod, payLoad);
        }

        public string GetVuidFromCookieValue(string cookieValue)
        {

            if (!string.IsNullOrWhiteSpace(cookieValue) && cookieValue.Length > 35)
            {
                return cookieValue.Substring(0, 36).Replace("-", string.Empty);
            }

            return null!;

        }

        public string GetVuidFromHttpRequest(HttpRequest httpRequest)
        {
            return GetVuidFromHttpRequest(httpRequest.Cookies);
        }

        public string GetVuidFromHttpRequest(IRequestCookieCollection cookies)
        {
            if (cookies.TryGetValue("vuid", out var vuid))
            {
                return GetVuidFromCookieValue(vuid!); // Could still be null
            }
            return null!;
        }

        private string SerializeToJson(object obj)
        {
            var settings = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            };
            var json = JsonSerializer.Serialize(obj, settings);
            return json;
        }


    }
}
