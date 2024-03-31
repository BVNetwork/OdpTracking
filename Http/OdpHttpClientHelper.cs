using System.Net;
using System.Text;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using OdpTracking.Configuration;

namespace OdpTracking.Http
{
    public class OdpHttpClientHelper : IOdpHttpClientHelper
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IOdpTrackerOptions _trackerOptions;
        private readonly ILogger<OdpHttpClientHelper> _logger;

        public OdpHttpClientHelper(IHttpClientFactory httpClientFactory,
            IOdpTrackerOptions trackerOptions,
            ILogger<OdpHttpClientHelper> logger)
        {
            _httpClientFactory = httpClientFactory;
            _trackerOptions = trackerOptions;
            _logger = logger;
        }

        public T GetJson<T>(string apiMethod, string query, string payLoad)
        {
            bool success = TryMakeHttpCall(HttpMethod.Get,
                apiMethod, query,
                payLoad, out HttpStatusCode statusCode,
                out var response);
            if (success)
            {
                _logger.LogDebug("Successfull {method} to: {name}, result: {status}",
                    HttpMethod.Get,
                    apiMethod,
                    statusCode
                );
                using var reader = new StreamReader(response.Content.ReadAsStream());
                var resultString = reader.ReadToEnd();

                T resultObject = JsonConvert.DeserializeObject<T>(resultString);
                return resultObject;
            }
            else
            {
                _logger.LogWarning("Error {method} to: {name}, result: {status}",
                    HttpMethod.Post,
                    apiMethod,
                    statusCode
                );
            }

            return default(T);
        }

        public HttpStatusCode PostJson(string apiMethod, string payLoad)
        {
            bool success = TryMakeHttpCall(HttpMethod.Post,
                apiMethod, null,
                payLoad, out HttpStatusCode statusCode,
                out var response);
            if (success)
            {
                _logger.LogDebug("Successfull {method} to: {name}, result: {status}",
                    HttpMethod.Post,
                    apiMethod,
                    statusCode
                );
            }
            else
            {
                _logger.LogWarning("Error {method} to: {name}, result: {status}",
                    HttpMethod.Post,
                    apiMethod,
                    statusCode
                );
            }
            
            return response.StatusCode;
        }

        protected bool TryMakeHttpCall(HttpMethod httpMethod, string apiMethod, string query,
            string payLoad, out HttpStatusCode statusCode, out HttpResponseMessage responseMessage)
        {
            var httpClient = _httpClientFactory.CreateClient("OdpTracker");
            var uri = CreateUri(_trackerOptions.BaseUrl, apiMethod, query);

            var requestMsg = new HttpRequestMessage(httpMethod, uri);
            if(payLoad != null)
            {
                var content = new StringContent(payLoad, Encoding.UTF8, "application/json");
                requestMsg.Content = content;
            }
            requestMsg.Headers.Add("x-api-key", _trackerOptions.PrivateKey);
            // This might be a little short, but we don't want the site to wait
            httpClient.Timeout = new TimeSpan(0, 0, 0, 5);

            HttpResponseMessage response = null;
            try
            {
                response = httpClient.Send(requestMsg);
            }
            catch (HttpRequestException e)
            {
                statusCode = response != null ? response.StatusCode : 0;
                _logger.LogError(e, "Error posting to: {name}, result: {status}",
                    uri.ToString(),
                    statusCode);
                responseMessage = null;
                return false;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error making Http call to: {name}",
                    uri.ToString());
                responseMessage = null;
                statusCode = 0;
                return false;
            }

            statusCode = response.StatusCode;
            responseMessage = response;
            // using var reader = new StreamReader(response.Content.ReadAsStream());
            // var resultString = reader.ReadToEnd();

            return response.IsSuccessStatusCode;
        }
        
        public Uri CreateUri(string baseUrl, string path, string query = null)
        {
            if(string.IsNullOrEmpty(path)) throw new ArgumentNullException(nameof(path), "Path cannot be null");
            
            string url = baseUrl;
            if (path.StartsWith('/') == false)
            {
                url = url + '/' + path;
            }
            else
            {
                url = url + path;
            }

            if (string.IsNullOrEmpty(query) == false)
            {
                if (query.StartsWith('?') == false)
                {
                    url = url + '?' + query;
                }
                else
                {
                    url = url + query;
                }
            }

            return new Uri(url);
        }
    }
}