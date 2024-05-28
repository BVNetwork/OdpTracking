# OdpTracking
This library helps doing both server and client side tracking to Optimizely Data Platform.

**Note** This library implements a "forgiving API", which means it will attempt to return valid, but empty results instead of throwing argument exceptions. If you forget to pass a parameter, or pass a null value, you will most likely not get an error, but no tracking will happen and no results will be returned. This is to prevent coding errors or missing input validation to crash the site, as this type of functionality is typically not crucial for the site operation.

## Installation

Install the package directly from the Optimizely Nuget repository.

``` 
dotnet add package OdpTracking
```
```
Install-Package OdpTracking
```

## Configuration (.NET 6.0+)
You need to configure the following application settings (these are at the root of your `appsettings.json` file):

````json
{
  "OdpPrivateKey": "DFLo0X7...",
  "OdpApiBaseUrl": "https://api.zaius.com",
  "OdpTrackingId": "..."
}
````

You can find the `OdpPrivateKey` in your ODP settings:
1. Go to Account Settings > Data Management > APIs
1. Click Private
1. Copy the Key (Do not click Revoke & Generate New Key unintentionally)

The `OdpApiBaseUrl` is the regional base url. It is typically one of these:
* US – https://api.zaius.com
* Europe – https://api.eu1.odp.optimizely.com
* Asia-Pacific (APAC) – https://api.au1.odp.optimizely.com

`OdpTrackingId` is the JavaScript tracking ID from the JavaScript integration page in ODP:
1. Go to Account Settings > Data Management > Integrations
1. Click JavaScript Tag.
1. Copy the "Tracker ID"

*Important!* You do not need to copy the ODP JavaScript itself, it will be included on all pages by this library if you use the Client Resource tag helper or MVC Html helper:

Using the tag helper:
```html
<html>
    <head>
        <required-client-resources area="Header"/>
    </head>
    <body>
        ...
    </body>
</html>
```
Using the HTML helper:
```html
<html>
    <head>
        @Html.RequiredClientResources("Header")
    </head>
    <body>
        ...
    </body>
</html>
```

### Startup

In `startup.cs` add:
```csharp
public void ConfigureServices(IServiceCollection services)
{
    ...
    services.AddOdpTracking();
    ...
}
```

## Multisite
If you do not want to store the tracker id in the configuration file (default implementation in `ConfigurationTrackerIdProvider`), you can store it somewhere else. Typically you'd want to store the tracker id on a property on your start page, or a site specific configuration page. In order for the client resource provider to be able to render the tracking script, you need to provide the tracker ID before the Client Resource provider is rendering the script.

You can implement your own ITrackerProvider to return the tracker id, or use the `HttpContextOdpTrackerIdProvider`. It will retrieve the Tracker ID from the `HttpContext` during rendering, and you are responsible for adding it to `HttpContext` during the request.  

To use the `HttpContextOdpTrackerIdProvider`, register it during startup:
```csharp
public void ConfigureServices(IServiceCollection services)
{
    ...
    services.AddOdpTracking();
    // Override how we provide the tracker id
    services.AddTransient<ITrackerIdProvider, HttpContextOdpTrackerIdProvider>();
}
```
If you have a similar setup as Alloy, you would typically do this in the `PageViewContextFactory`:
```csharp
public class PageViewContextFactory
{
    private readonly ITrackerIdProvider _trackerIdProvider;

    public PageViewContextFactory(ITrackerIdProvider trackerIdProvider)
    {
        // Other dependencies removed for simplicity
        _trackerIdProvider = trackerIdProvider;
    }

    public virtual LayoutModel CreateLayoutModel(ContentReference currentContentLink, HttpContext httpContext)
    {
        var startPageContentLink = SiteDefinition.Current.StartPage;
        var startPage = _contentLoader.Get<StartPage>(startPageContentLink);

        // The ODP Tracking script will pick this up later, read it from the start page
        _trackerIdProvider.SetTrackerIdForRequest(httpContext, startPage.OdpTrackerId);
        ...
    }
}
```

## Server Side Tracking
### VUID
The `VUID` is a cookie value that identifies the current user. If there is no previous `VUID`, a new one is generated. The value in this cookie is passed with all client side tracking calls, and is also picked up by the server side tracking calls.

There are helper methods to get a proper `VUID`  from code.

### Example: Tracking Login
```csharp
using Microsoft.AspNetCore.Mvc;
using EPiServer.Cms.UI.AspNetIdentity;
using Microsoft.AspNetCore.Identity;
using OdpTracking;
using OdpTracking.Extensions;

namespace AlloyTours.Controllers
{
    public class AccountController : Controller
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IOdpServerSideTracker _serverSideTracker;

        public AccountController(SignInManager<ApplicationUser> signInManager, IOdpServerSideTracker serverSideTracker)
        {
            _signInManager = signInManager;
            _serverSideTracker = serverSideTracker;
        }
        
        // POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("{controller}/login")]
        public async Task<IActionResult> Login(string email, string password, bool rememberMe)
        {
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(email, password, rememberMe,
                    lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    var vuid = this.Request.GetVuid();
                    _serverSideTracker.TrackLogin(email, vuid);
                    return Redirect("/");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                }
            }
            return Redirect("/");
        }
    }
}
```
The tracking part is this:
```csharp
var vuid = this.Request.GetVuid();
_serverSideTracker.TrackLogin(email, vuid);
```
The `GetVuid` method is one of two in the `VuidExtensions` class to help parse the `VUID` from the Request Cookies. You can read the `VUID` from the `HttpRequest` or `IRequestCookieCollection`.

The `_serverSideTracker` implements `IOdpServerSideTracker`, which has several methods to help track different events and send them to the ODP HTTP SDK, like the `TrackLogin`.

In this case, when we track the login, ODP will stitch the email address and the `VUID` together with previous sessions with other `VUIDs`, creating one combined profile across sessions.

Behind the scenes, the following data will be sent to the [ODP REST API](https://docs.developers.optimizely.com/optimizely-data-platform/reference/introduction):
```
POST /v3/events HTTP/1.1
Content-Type: application/json
Host: api.zaius.com
```
```json
{
    "type": "account",
    "action": "login",
    "identifiers": {
        "email": "customer-email-address@customer-domain.com",
        "vuid": "6067f061d0f04b76812dc0308270a6e2"
    }
}   
```
## Client Side Tracking
This library also has features to help create client side JavaScript to do tracking.

### Example: Add to wish list
In this example, using the `ProductEvent` class and the `AddToWishList` static method will return the necessary JavaScript to track that someone adds a product to their Wish List.
```html
<a href="#" onclick="@ProductEvent.AddToWishlist(Model.CurrentPage.Sku).GetJavascriptCall()">
    Add to Wishlist
</a>
```
It produces the following JavaScript if the Model.CurrentPage.Sku has a value:
```html
<a href="#" onclick="zaius.event('product', {action: 'add_to_wishlist', product_id: 'some-product-id'});">
    Add to Wishlist
</a>
```

It produces the following JavaScript if the Model.CurrentPage.Sku is null or empty:
```html
<a href="#" onclick="">
    Add to Wishlist
</a>
```
Relevant client tracking classes:
* AccountEvent
  * Logout()
* ProductEvent
  * AddToWishlist(product_id)
  * Detail(product_id)
* SearchEvent
  * Search(search_term) 

