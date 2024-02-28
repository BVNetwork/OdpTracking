using EPiServer.Framework.Web.Resources;

namespace OdpTracking
{
    [ClientResourceRegistrator]
    public class OdpTrackerClientResourceRegistrator : IClientResourceRegistrator
    {
        private static string ScriptKey = "contentpersonalization.script.recommendations";

        public void RegisterResources(IRequiredClientResourceList requiredResources)
        {
            requiredResources.Require(ScriptKey).AtHeader();
        }
    }


    [ClientResourceProvider]
    public class OdpTrackerClientResourceProvider : IClientResourceProvider
    {
        private readonly ITrackerIdProvider _idProvider;
        private static string ScriptKey = "contentpersonalization.script.recommendations";

        public OdpTrackerClientResourceProvider(ITrackerIdProvider idProvider)
        {
            _idProvider = idProvider;
        }

        public IEnumerable<ClientResource> GetClientResources()
        {
            var trackerId = _idProvider.GetTrackerId();
            if (string.IsNullOrEmpty(trackerId) == false)
            {
                return (IEnumerable<ClientResource>)new ClientResource[1]
                {
                    new ClientResource()
                    {
                        Name = ScriptKey,
                        ResourceType = ClientResourceType.Script,
                        // Adds correct tracking id to script
                        InlineContent =
                            "var zaius = window['zaius']||(window['zaius']=[]);zaius.methods=[\"initialize\",\"onload\",\"customer\",\"entity\",\"event\",\"subscribe\",\"unsubscribe\",\"consent\",\"identify\",\"anonymize\",\"dispatch\"];zaius.factory=function(e){return function(){var t=Array.prototype.slice.call(arguments);t.unshift(e);zaius.push(t);return zaius}};(function(){for(var i=0;i<zaius.methods.length;i++){var method=zaius.methods[i];zaius[method]=zaius.factory(method)}var e=document.createElement(\"script\");e.type=\"text/javascript\";e.async=true;e.src=(\"https:\"===document.location.protocol?\"https://\":\"http://\")+\"d1igp3oop3iho5.cloudfront.net/v2/" + trackerId + "/zaius-min.js\";var t=document.getElementsByTagName(\"script\")[0];t.parentNode.insertBefore(e,t)})();\n  \n  // Edits to this script should only be made below this line.\n  zaius.event('pageview');\n"
                    }
                };
            }
            return Enumerable.Empty<ClientResource>();
        }
    }
}