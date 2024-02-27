#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
using System.Text.Json.Serialization;

namespace OdpTracking.Dto
{
    public class OdpDtoOrder
    {
        [JsonPropertyName("order_id")]
        public string OrderId { get; set; }
        public decimal Total { get; set; }
        public IEnumerable<OdpDtoLineItem> Items { get; set; }
        public decimal Subtotal { get; set; }
    }
}