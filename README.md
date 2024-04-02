# OdpTracking

Tracking library for Optimizely Data Platform.

This library helps doing both server and client side tracking to ODP.

## Getting Started
You need to configure the following application settings (these are at the root of your `appsettings.json` file):
### Configuration
````json
{
  "OdpPrivateKey": "DFLo0X7...",
  "OdpApiBaseUrl": "https://api.zaius.com",
  "OdpTrackingId": "..."
}
````
OdpTrackingId is the JavaScript tracking ID that you can find in the JavaScript traking id

You can find the private key in your ODP settings. The OdpApiBaseUrl is the regional base url. It is typically one of these:
* US – https://api.zaius.com
* Europe – https://api.eu1.odp.optimizely.com
* Asia-Pacific (APAC) – https://api.au1.odp.optimizely.com

### Startup

In `startup.cs` add:
```csharp
public void ConfigureServices(IServiceCollection services)
{
    services.AddOdpTracking();    
}
```
