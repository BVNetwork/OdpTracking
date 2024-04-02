using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;
using Microsoft.AspNetCore.Html;

namespace OdpTracking.Client
{
    public abstract class OdpClientEvent
    {
        public virtual string Type { get; }
        public virtual string Action { get; set; }
        
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static string GetCurrentMethod()
        {
            var st = new StackTrace();
            var sf = st.GetFrame(1);

            return sf.GetMethod().Name;
        }

        public virtual HtmlString GetJavascriptCall()
        {
            return GetJavascriptCall(null);
        }
        
        public virtual HtmlString GetJavascriptCall(IList<KeyValuePair<string, string>> optionalParams)
        {
            var js = new StringBuilder();
            var action = new StringBuilder().AppendFormat("action: '{0}'", Action.ToLower());
            if(optionalParams != null && optionalParams.Count != -1)
            {
                foreach (var param in optionalParams)
                {
                    action.AppendFormat(", {0}: '{1}'", param.Key, param.Value);
                }
            }
            
            js.AppendFormat("zaius.event('{0}', {{{1}}});", Type.ToLower(), action.ToString());
            return new HtmlString(js.ToString());
        }
    }

    public class PageViewEvent : OdpClientEvent
    {
        public override string Type { get; } = OdpClientSideEventTypes.Pageview.ToString();
    }
    
    public class AccountEvent : OdpClientEvent
    {
        public override string Type { get; } = OdpClientSideEventTypes.Account.ToString();

        public static AccountEvent Login() => new() { Action = GetCurrentMethod() };
        public static AccountEvent Logout() => new() { Action = GetCurrentMethod() };
        public static AccountEvent Register() => new() { Action = GetCurrentMethod() };
        public static AccountEvent Update() => new() { Action = GetCurrentMethod() };
    }
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
    
    public class SearchEvent : OdpClientEvent
    {
        public virtual string SearchTerm { get; set; }
        public override string Type { get; } = OdpClientSideEventTypes.Search.ToString();
        public static SearchEvent Search(string searchTerm) => new()
        {
            Action = GetCurrentMethod(),
            SearchTerm = searchTerm
        };

        public override HtmlString GetJavascriptCall()
        {
            // Forgiving implementation
            if(SearchTerm == null) return HtmlString.Empty;
            
            return base.GetJavascriptCall(new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("search_term", SearchTerm)
            });
        }
    }

    public enum OdpClientSideEventTypes
    {
        Account,
        Pageview,
        Product,
        Order,
        Email,
        List,
        Consent,
        Reachability,
        Web_Modal,
        Web_Embed,
        Search
    }
}