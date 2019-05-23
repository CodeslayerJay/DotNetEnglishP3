using Microsoft.Net.Http.Headers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using CookieHeaderValue = System.Net.Http.Headers.CookieHeaderValue;

namespace P3AddNewFunctionalityDotNetCore.IntegrationTests.Helpers
{
    /// <summary>
    /// Help extracting and creating cookies for testing purposes
    /// Reference: https://www.stefanhendriks.com/2016/05/11/integration-testing-your-asp-net-core-app-dealing-with-anti-request-forgery-csrf-formdata-and-cookies/
    /// </summary>
    public static class CookiesHelper
    {
        

        public static IDictionary<string, string> ExtractCookiesFromResponse(HttpResponseMessage response)
        {
            IDictionary<string, string> result = new Dictionary<string, string>();
            IEnumerable<string> values;
            if (response.Headers.TryGetValues("Set-Cookie", out values))
            {
                SetCookieHeaderValue.ParseList(values.ToList()).ToList().ForEach(cookie =>
                {
                    result.Add(cookie.Name.ToString(), cookie.Value.ToString());
                });
            }
            return result;
        }

        public static HttpRequestMessage PutCookiesOnRequest(HttpRequestMessage request, IDictionary<string, string> cookies)
        {
            cookies.Keys.ToList().ForEach(key =>
            {
                request.Headers.Add("Cookie", new CookieHeaderValue(key, cookies[key]).ToString());
            });

            return request;
        }

        public static HttpRequestMessage CopyCookiesFromResponse(HttpRequestMessage request, HttpResponseMessage response)
        {
            return PutCookiesOnRequest(request, ExtractCookiesFromResponse(response));
        }
    }
}

