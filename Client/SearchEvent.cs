using Microsoft.AspNetCore.Html;

namespace OdpTracking.Client
{
    public class SearchEvent : OdpClientEvent
    {
        public virtual string SearchTerm { get; set; }
        public override string Type { get; } = OdpClientSideEventTypes.Navigation.ToString();
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
}