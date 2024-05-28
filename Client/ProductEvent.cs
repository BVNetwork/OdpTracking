using Microsoft.AspNetCore.Html;

namespace OdpTracking.Client
{
    public class ProductEvent : OdpClientEvent
    {
        public virtual string ProductId { get; set; }
        public override string Type { get; } = OdpClientSideEventTypes.Product.ToString();
        public static ProductEvent AddToWishlist(string productId) => new()
        {
            Action = "add_to_wishlist",
            ProductId = productId
        };
        public static ProductEvent Detail(string productId) => new()
        {
            Action = GetCurrentMethod(),
            ProductId = productId
        };
        public override HtmlString GetJavascriptCall()
        {
            if (ProductId == null) return HtmlString.Empty;
            
            return base.GetJavascriptCall(new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("product_id", ProductId)
            });
        }
    }
}