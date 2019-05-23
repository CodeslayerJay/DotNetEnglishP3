using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace P3AddNewFunctionalityDotNetCore.IntegrationTests.Helpers
{
    /// <summary>
    /// Helper class for dealing with antiforgery, tokens, etc.
    /// Reference: https://geeklearning.io/asp-net-core-mvc-testing-and-the-synchronizer-token-pattern/
    /// </summary>
    public static class ClientHelpers
    {
        /// <summary>
        /// Retrieve view data, extract Antiforgery token, then attempt to
        /// post to url with token injected into the request.
        /// </summary>
        /// <typeparam name="TContent"></typeparam>
        /// <param name="httpClient"></param>
        /// <param name="requestUri"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        public static async Task<HttpResponseMessage> PostAntiForgeryAsync(this HttpClient httpClient, string requestUri, 
            IDictionary<string, string> formData)
        {
            // Get the form view
            HttpResponseMessage responseMsg = await httpClient.GetAsync(requestUri);
            if (!responseMsg.IsSuccessStatusCode)
            {
                return responseMsg;
            }

            // Extract Anti Forgery Token
            var antiForgeryToken = await ExtractAntiForgeryTokenAsync(responseMsg);

            // Serialize data to Key/Value pairs
            IDictionary<string, string> contentData = formData;

            // Create the request message with previously serialized data + the Anti Forgery Token
            contentData.Add("__RequestVerificationToken", antiForgeryToken);
            var requestMsg = new HttpRequestMessage(HttpMethod.Post, requestUri)
            {
                Content = new FormUrlEncodedContent(contentData)
            };

            // Copy the cookies from the response (containing the Anti Forgery Token) to the request that is about to be sent
            requestMsg = CookiesHelper.CopyCookiesFromResponse(requestMsg, responseMsg);

            return await httpClient.SendAsync(requestMsg);
        }

        public static string ExtractAntiForgeryToken(string htmlResponseText)
        {
            if (htmlResponseText == null) throw new ArgumentNullException("htmlResponseText");

            System.Text.RegularExpressions.Match match = Regex.Match(htmlResponseText, @"\<input name=""__RequestVerificationToken"" type=""hidden"" value=""([^""]+)"" \/\>");
            return match.Success ? match.Groups[1].Captures[0].Value : null;
        }

        public static async Task<string> ExtractAntiForgeryTokenAsync(HttpResponseMessage response)
        {
            string responseAsString = await response.Content.ReadAsStringAsync();
            return await Task.FromResult(ExtractAntiForgeryToken(responseAsString));
        }
    }
}
