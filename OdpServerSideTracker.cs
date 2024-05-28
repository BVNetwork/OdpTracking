using System.Net;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using OdpTracking.Configuration;
using OdpTracking.Dto;
using OdpTracking.Extensions;
using OdpTracking.Http;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace OdpTracking
{
    /// <summary>
    /// Main ODP server tracker implementation. Use the IOdpServerSideTracker through
    /// dependency injection to get the correct implementation.
    /// </summary>
    public class OdpServerSideTracker : IOdpServerSideTracker
    {
        private const string ApiProfilesUrlSegment = "/v3/profiles";
        private const string ApiProductsUrlSegment = "/v3/objects/products";
        private const string ApiEventsUrlSegment = "/v3/events";
        private readonly ILogger<OdpServerSideTracker> _logger;
        private readonly IOdpHttpClientHelper _httpClientHelper;
        private readonly IOdpTrackerOptions _odpTrackerOptions;

        public OdpServerSideTracker(
            ILogger<OdpServerSideTracker> logger,
            IOdpHttpClientHelper httpClientHelper,
            IOdpTrackerOptions odpTrackerOptions)
        {
            _logger = logger;
            _httpClientHelper = httpClientHelper;
            _odpTrackerOptions = odpTrackerOptions;
        }

        public void TrackLogin(string email, string vuid)
        {
            TrackSingleEvent(email, vuid, "account", "login");
        }

        public void TrackLogout(string email, string vuid)
        {
            TrackSingleEvent(email, vuid, "account", "logout");
        }

        public void TrackSingleEvent(string email, string vuid, string type, string action)
        {
            if (_odpTrackerOptions.IsConfigured() == false)
                return;

            if (HasEmailAndVuid(email, vuid))
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
                    Action = action,
                    Type = type,
                    Identifiers = new OdpDtoIdentifiers
                    {
                        Email = EncodeEmail(email),
                        Vuid = vuid
                    }
                };
                TrackEvent(data);
            }
        }

        public void TrackOrder(string email, string vuid, OdpDtoOrder order)
        {
            if (_odpTrackerOptions.IsConfigured() == false)
                return;

            if (HasEmailAndVuid(email, vuid))
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
                        Email = EncodeEmail(email),
                        Vuid = vuid
                    },
                    Data = new { Order = order }
                };
                string payLoad = SerializeToJson(data);
                _logger.LogDebug("Tracking order: " + order.OrderId);
                _logger.LogDebug(payLoad);

                PostPayload(payLoad, ApiEventsUrlSegment);
            }
        }

        public void TrackEvent(OdpDtoEvent odpEvent)
        {
            if (_odpTrackerOptions.IsConfigured() == false)
                return;

            if (string.IsNullOrEmpty(odpEvent.Identifiers.Vuid) == false)
            {
                string payLoad = SerializeToJson(odpEvent);
                PostPayload(payLoad, ApiEventsUrlSegment);
            }
        }

        /// <summary>
        /// Connects Vuid, email and other fields. Useful when you do not have
        /// any other information about the profile, or you want
        /// to connect a new vuid to an email.
        /// </summary>
        /// <param name="email"></param>
        /// <param name="vuid"></param>
        /// <param name="firstName"></param>
        /// <param name="lastName"></param>
        public void UpdateProfile(string vuid, string email, string firstName = null, string lastName = null)
        {
            var profile = new OdpDtoProfile
            {
                Vuid = vuid,
                Email = EncodeEmail(email),
                FirstName = firstName,
                LastName = lastName
            };
            UpdateProfile(profile);
        }

        public void UpdateProfile(OdpDtoProfile profile)
        {
            if (_odpTrackerOptions.IsConfigured() == false)
                return;

            if (HasEmailAndVuid(profile.Email, profile.Vuid))
            {
                var payLoad = SerializeToJson(new { Attributes = profile });

                // {
                //     attributes:
                //     {
                //         email: " + email + @",
                //         vuid: " + vuid + @" 
                //     }
                // }

                PostPayload(payLoad, ApiProfilesUrlSegment);
            }
        }

        public void UploadProducts(IEnumerable<OdpDtoProduct> products)
        {
            var payLoad = SerializeToJson(products);
            PostPayload(payLoad, ApiProductsUrlSegment);
        }

        private void PostPayload(string payLoad, string apiMethod)
        {
            var statusCode = _httpClientHelper.PostJson(apiMethod, payLoad);
            if (statusCode.IsSuccessStatusCode() == false)
            {
                _logger.LogError("Posting payload to {apiMethod} failed with status code: {statusCode}", apiMethod,
                    statusCode);
            }
        }

        public bool HasEmailAndVuid(string email, string vuid)
        {
            if (string.IsNullOrEmpty(vuid) == false &&
                IsValidEmail(email))
            {
                return true;
            }

            return false;
        }

        public bool IsValidEmail(string email)
        {
            if (string.IsNullOrEmpty(email))
                return false;

            return email.Contains('@');
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

        private string EncodeEmail(string email)
        {
            // Product Recs decodes the email address in the query string
            // and replaces "+" with " ". Let's add that back
            return email.Replace(" ", "+");
        }
    }
}