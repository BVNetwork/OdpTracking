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
}