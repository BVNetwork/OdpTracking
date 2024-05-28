namespace OdpTracking.Client
{
    public class PageViewEvent : OdpClientEvent
    {
        public override string Type { get; } = OdpClientSideEventTypes.Pageview.ToString();
    }
}