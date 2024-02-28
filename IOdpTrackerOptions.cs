namespace OdpTracking
{
    public interface IOdpTrackerOptions
    {
        public string PrivateKey { get; }
        public string BaseUrl{ get; }
        public bool IsConfigured();
        public Uri CreateUri(string path, string query = null);
    }
}