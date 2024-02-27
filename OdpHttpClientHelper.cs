﻿using System.Net;
using System.Text;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace OdpTracking
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

        public T? GetJson<T>(string apiMethod, string query, string payLoad)
        {
            HttpResponseMessage response = null;
            bool success = TryMakeHttpCall(HttpMethod.Get,
                apiMethod, query,
                payLoad, out HttpStatusCode statusCode,
                out response);
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
            HttpResponseMessage response = null;
            bool success = TryMakeHttpCall(HttpMethod.Post,
                apiMethod, null,
                payLoad, out HttpStatusCode statusCode,
                out response);
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

        private bool TryMakeHttpCall(HttpMethod httpMethod, string apiMethod, string query,
            string payLoad, out HttpStatusCode statusCode, out HttpResponseMessage responseMessage)
        {
            var httpClient = _httpClientFactory.CreateClient("OdpTracker");
            var uri = _trackerOptions.CreateUri(apiMethod, query);

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
            catch (System.Net.Http.HttpRequestException e)
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
    }
}