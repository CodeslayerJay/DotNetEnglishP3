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
        // User login details
        private const string AdminUser = "Admin";
        private const string AdminPassword = "P@ssword123";
        private const string LoginRoute = "/Account/Login";

        // Login with test credentials
        public static async Task<HttpResponseMessage> LoginAsAdminAsync(this HttpClient httpClient)
        {
            // Login as administrator
            var loginDetails = new Dictionary<string, string> { { "Name", AdminUser }, { "Password", AdminPassword } };

            var response = await PostAntiForgeryAsync(httpClient, LoginRoute, loginDetails);
            response.EnsureSuccessStatusCode();

            return response;
        }

        /// <summary>
        /// Make a post request with antiforgerytoken
        /// </summary>
        /// <param name="httpClient"></param>
        /// <param name="requestUri"></param>
        /// <param name="formData"></param>
        /// <returns></returns>
        public static async Task<HttpResponseMessage> PostAntiForgeryAsync(this HttpClient httpClient, string formRequestUri, 
            IDictionary<string, string> formData)
        {
            // Get the form view
            HttpResponseMessage responseMsg = await httpClient.GetAsync(formRequestUri);
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
            var requestMsg = new HttpRequestMessage(HttpMethod.Post, formRequestUri)
            {
                Content = new FormUrlEncodedContent(contentData)
            };

            // Copy the cookies from the response (containing the Anti Forgery Token) to the request that is about to be sent
            //requestMsg = CookiesHelper.CopyCookiesFromResponse(requestMsg, responseMsg);

            return await httpClient.SendAsync(requestMsg);
        }

        public static async Task<HttpResponseMessage> PostAntiForgeryAsync(this HttpClient httpClient, 
            string formRequestUri, string postRequestUri,
            IDictionary<string, string> formData)
        {
            // Get the form view
            HttpResponseMessage responseMsg = await httpClient.GetAsync(formRequestUri);
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
            var requestMsg = new HttpRequestMessage(HttpMethod.Post, postRequestUri)
            {
                Content = new FormUrlEncodedContent(contentData)
            };
            
            return await httpClient.SendAsync(requestMsg);
        }


        /// <summary>
        /// Make a post request as an authorized user (async)
        /// </summary>
        public static async Task<HttpResponseMessage> PostWithAuthAsync(this HttpClient httpClient, string formRequestUri,
            IDictionary<string, string> formData)
        {

            await LoginAsAdminAsync(httpClient);

            return await PostAntiForgeryAsync(httpClient, formRequestUri, formData);
        }

        
        public static async Task<HttpResponseMessage> PostWithAuthAsync(this HttpClient httpClient, 
            string formRequestUri, string postRequestUri,
            IDictionary<string, string> formData)
        {

            await LoginAsAdminAsync(httpClient);

            return await PostAntiForgeryAsync(httpClient, formRequestUri, postRequestUri, formData);
        }

        /// <summary>
        /// Get request as an authorized user
        /// </summary>
        /// <param name="httpClient"></param>
        /// <param name="requestUri"></param>
        /// <returns></returns>
        public static async Task<HttpResponseMessage> GetAsAuthAsync(this HttpClient httpClient, string requestUri)
        {
            await LoginAsAdminAsync(httpClient);

            return await httpClient.GetAsync(requestUri);
        }


        /// <summary>
        /// Extract anti forgery token from response
        /// </summary>
        /// <param name="htmlResponseText"></param>
        /// <returns></returns>
        public static string ExtractAntiForgeryToken(string htmlResponseText)
        {
            if (htmlResponseText == null) throw new ArgumentNullException(nameof(htmlResponseText));

            System.Text.RegularExpressions.Match match = Regex.Match(htmlResponseText, @"\<input name=""__RequestVerificationToken"" type=""hidden"" value=""([^""]+)"" \/\>");
            return match.Success ? match.Groups[1].Captures[0].Value : null;
        }

        /// <summary>
        /// Extract anti forgery token from response (async)
        /// </summary>
        /// <param name="response"></param>
        /// <returns></returns>
        public static async Task<string> ExtractAntiForgeryTokenAsync(HttpResponseMessage response)
        {
            string responseAsString = await response.Content.ReadAsStringAsync();
            return await Task.FromResult(ExtractAntiForgeryToken(responseAsString));
        }
    }
}
