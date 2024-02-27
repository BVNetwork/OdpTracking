#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace OdpTracking.Dto
{
    public class OdpDtoIdentifiers
    {
        [JsonRequired]
        public string Vuid { get; set; }
        [System.Text.Json.Serialization.JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string Email { get; set; }
    }
}