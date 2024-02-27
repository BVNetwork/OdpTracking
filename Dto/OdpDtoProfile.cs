#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
using System.Text.Json.Serialization;

namespace OdpTracking.Dto
{
    public class OdpDtoProfile
    {
        public string Email { get; set; }

        [JsonPropertyName("first_name")] public string FirstName { get; set; }

        [JsonPropertyName("last_name")] public string LastName { get; set; }
        public string Phone { get; set; }
        public string Vuid { get; set; }
        public string Street1 { get; set; }
        public string City { get; set; }
        public string Zip { get; set; }
    }
}