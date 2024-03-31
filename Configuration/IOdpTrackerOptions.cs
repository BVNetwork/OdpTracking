namespace OdpTracking.Configuration
{
    public interface IOdpTrackerOptions
    {
        public string PrivateKey { get; }
        public string BaseUrl{ get; }
        public bool IsConfigured();
    }
}