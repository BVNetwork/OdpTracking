#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

using System.Text.Json.Serialization;

namespace OdpTracking.Dto
{
    public class OdpDtoProduct
    {
        [JsonPropertyName("product_id")]
        public string Id { get; set; }
        [JsonPropertyName("parent_product_id")]
        public string ParentId { get; set; }

        public string Name { get; set; }
        public string Brand { get; set; }
        public string Sku { get; set; }
        public string Upc { get; set; }
        [JsonPropertyName("image_url")]
        public string ImageUrl { get; set; }
        [JsonPropertyName("product_url")]
        public string ProductUrl { get; set; }
        public string Price { get; set; }

        // public static OdpDtoProduct FromFindProduct(FindProduct product)
        // {
        //     OdpDtoProduct odpProduct = new OdpDtoProduct
        //     {
        //         Name = product.Name,
        //         Brand = product.Brand,
        //         Id = product.Code,
        //         // Needs to be fully qualified - TODO: Get from site settings
        //         ImageUrl = "https://www.epicphoto.no" + product.DefaultImageUrl,
        //         ProductUrl = product.ProductUrl,
        //         Sku = product.Code,
        //         Upc = product.DescriptionString,
        //         Price = product.DefaultPriceAmount.ToString()
        //     };
        //
        //     return odpProduct;
        // }

    }
}