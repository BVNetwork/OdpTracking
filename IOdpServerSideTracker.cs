using OdpTracking.Dto;

namespace OdpTracking
{
    /// <summary>
    /// Main class for tracking to ODP. Use dependency injection to get an implementation instance.
    /// </summary>
    /// <remarks>Use the extensions method in the Extensions namespace to help read the VUID</remarks>
    public interface IOdpServerSideTracker
    {
        void TrackLogin(string email, string vuid);
        void TrackOrder(string email, string vuid, OdpDtoOrder order);
        void TrackEvent(OdpDtoEvent odpEvent);
        void UpdateProfile(OdpDtoProfile profile);
        void UpdateProfile(string vuid, string email, string firstName = null, string lastName = null);
        void UploadProducts(IEnumerable<OdpDtoProduct> products);
    }


}