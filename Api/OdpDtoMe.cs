using Newtonsoft.Json;
using OdpTracking.Dto;
#pragma warning disable CS8618

namespace OdpTracking.Api
{
    public class OdpDtoMe
    {
        [JsonProperty(propertyName: "name")] public string Name { get; set; }
        [JsonProperty(propertyName: "email")] public string Email { get; set; }

        [JsonProperty(propertyName: "hasConsent")]
        public bool HasConsent { get; set; }

        [JsonProperty(propertyName: "firstname")]
        public string Firstname { get; set; }

        [JsonProperty(propertyName: "surname")]
        public string Surname { get; set; }
        
        [JsonProperty(propertyName: "isKnown")]
        public bool IsKnown { get; set; }

        public static OdpDtoMe FromProfile(OdpDtoCustomerProfile profile)
        {
            if(profile == null || profile.Attributes == null)
            {
                return new OdpDtoMe
                {
                    IsKnown = false
                };
            }
            
            OdpDtoMe me = new OdpDtoMe
            {
                Email = profile.Attributes.email,
                Firstname = profile.Attributes.first_name,
                Surname = profile.Attributes.last_name,
                Name = profile.Attributes.name,
                IsKnown = true
            };

            // We might have Name, but not first name, let's fix that
            if(string.IsNullOrEmpty(me.Firstname) &&
               string.IsNullOrEmpty(me.Name) == false)
            {
                // we pick the first one
                var nameParts = me.Name.Split(' ');
                me.Firstname = nameParts[0];
            } 
            
            // We might know the email, but that is not enough
            if (string.IsNullOrEmpty(me.Firstname))
                me.IsKnown = false;
            
            return me;
        }
            
    }
}