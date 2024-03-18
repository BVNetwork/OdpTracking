#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
using System.Text.Json.Serialization;

namespace OdpTracking.Dto
{
    public class OdpDtoLineItem
    {
        public OdpDtoLineItem()
        {
        }

        public OdpDtoLineItem(string id, decimal price, double quantity, string sku, decimal subtotal)
        {
            Id = id;
            Price = price;
            Quantity = quantity;
            Sku = sku;
            Subtotal = subtotal;
        }

        [JsonPropertyName("product_id")]
        public string Id { get; set; }

        public string Sku { get; set; }

        public double Quantity { get; set; }

        public decimal Price { get; set; }

        public decimal Subtotal { get; set; }
    }
}