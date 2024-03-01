using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using OdpTracking.Dto;

namespace OdpTracking
{
    public interface IOdpServerSideTracker
    {
        void TrackLogin(string email, string vuid);
        void TrackOrder(string email, string vuid, OdpDtoOrder order);
        string GetVuidFromCookieValue(string cookieValue);
        void TrackEvent(OdpDtoEvent odpEvent);
        void UpdateProfile(OdpDtoProfile profile);
        void UpdateProfile(string vuid, string email, string firstName = null, string lastName = null);
        void UploadProducts(IEnumerable<OdpDtoProduct> products);
    }


}