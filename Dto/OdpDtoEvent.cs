#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
namespace OdpTracking.Dto
{
    public class OdpDtoEvent
    {
        public string Type { get; set; }
        public string Action { get; set; }
        public OdpDtoIdentifiers Identifiers { get; set; }
        //public KeyValuePair<string, string> Data { get; set; }
        public object Data { get; set; }
    }
}