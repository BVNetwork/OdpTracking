using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using OdpTracking.Dto;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

namespace OdpTracking.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class OdpController : ControllerBase
    {
        private readonly ILogger<OdpController> _logger;
        private readonly IOdpServerSideTracker _odpServerSideTracker;
        private readonly IOdpHttpClientHelper _clientHelper;

        public OdpController(ILogger<OdpController> logger, IOdpServerSideTracker odpServerSideTracker, IOdpHttpClientHelper clientHelper)
        {
            _logger = logger;
            _odpServerSideTracker = odpServerSideTracker;
            _clientHelper = clientHelper;
        }


        [HttpGet("me")]
        public IActionResult Me()
        {
            // Pick up vuid from cookie
            var vuid = _odpServerSideTracker.GetVuidFromHttpRequest(Request);
            if(string.IsNullOrEmpty(vuid) == false)
            {
                var profile = _clientHelper.GetJson<OdpDtoCustomerProfile>("/v3/profiles", "vuid=" + vuid, null);
                var me = OdpDtoMe.FromProfile(profile);
                return Ok(me);
            }

            return Ok(new OdpDtoMe
            {
                IsKnown = false,
                HasConsent = false
            });
        }

        [HttpGet()]
        public IActionResult Get()
        {
            return Ok();
        }
    }
}