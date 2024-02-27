using System.Text.Json.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace OdpTracking.Dto
{
#pragma warning disable CS8618
    /// <summary>
    /// The profile object returned from the /v3/profiles call
    /// </summary>
    public class OdpDtoCustomerProfile
    {
        private string _jsonData;
        private JObject _profileJson;

        [JsonPropertyName("attributes")]
        public OdpDtoCustomerProfileAttributes Attributes { get; set; }

        internal void SetJsonData(string jsonData)
        {
            _jsonData = jsonData;
            _profileJson = JObject.Parse(jsonData);
        }
        
        public OdpDtoCustomerProfile FromJson(string jsonData)
        {
            var profile = JsonConvert.DeserializeObject<OdpDtoCustomerProfile>(jsonData);
            profile.SetJsonData(jsonData);
            return profile;
        }
        public bool TryGetAttribute(string name, out string ret)
        {
            var token = _profileJson["attributes"].FirstOrDefault(name);
            ret = token.ToString();;
            return false;
        } 
    }
    
    public class OdpDtoCustomerProfileAttributes
    {
        public string country { get; set; }
        public string gender { get; set; }
        public object city { get; set; }
        public string timezone { get; set; }
        public string data_source_version { get; set; }
        public object hubspot_139760765_id { get; set; }
        public int last_modified_at { get; set; }
        public string data_source_details { get; set; }
        public object street1 { get; set; }
        public string data_source_instance { get; set; }
        public string street2 { get; set; }
        public object state { get; set; }
        public string first_name { get; set; }
        public string email { get; set; }
        public object zip { get; set; }
        public object last_observed_timezone { get; set; }
        public object image_url { get; set; }
        public object ccpa_opted_out { get; set; }
        public string last_name { get; set; }
        public object dob_month { get; set; }
        public string data_source_type { get; set; }
        public string vuid { get; set; }
        public object commerce_cloud_id { get; set; }
        public string data_source { get; set; }
        public object dob_year { get; set; }
        public string phone { get; set; }
        public string name { get; set; }
        public object customer_id { get; set; }
        public object dob_day { get; set; }
    }
}