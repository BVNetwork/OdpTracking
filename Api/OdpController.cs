using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OdpTracking.Dto;
using OdpTracking.Extensions;
using OdpTracking.Http;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

namespace OdpTracking.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class OdpController : ControllerBase
    {
        private readonly ILogger<OdpController> _logger;
        private readonly IOdpHttpClientHelper _clientHelper;

        public OdpController(ILogger<OdpController> logger, IOdpHttpClientHelper clientHelper)
        {
            _logger = logger;
            _clientHelper = clientHelper;
        }


        [HttpGet("me")]
        public IActionResult Me()
        {
            // Pick up vuid from cookie
            var vuid = Request.GetVuid();
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