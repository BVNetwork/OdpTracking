using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using OdpTracking.Dto;

namespace OdpTracking
{
    public interface IOdpServerSideTracker
    {
        void DiscoverProfile(string email, string vuid);
        void TrackLogin(string email, string vuid);
        void TrackOrder(string email, string vuid, OdpDtoOrder order);
        string GetVuidFromCookieValue(string cookieValue);
        void TrackEvent(OdpDtoEvent odpEvent);
        void UpdateProfile(OdpDtoProfile profile);
        void UploadProducts(IEnumerable<OdpDtoProduct> products);
        string GetVuidFromHttpRequest(HttpRequest httpRequest);
        string GetVuidFromHttpRequest(IRequestCookieCollection cookies);
    }


}