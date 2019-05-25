using P3AddNewFunctionalityDotNetCore.IntegrationTests.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace P3AddNewFunctionalityDotNetCore.IntegrationTests
{
    public class AdminModuleCoherencyTests : IClassFixture<CustomWebApplicationFactory<P3AddNewFunctionalityDotNetCore.Startup>>
    {
        private readonly CustomWebApplicationFactory<Startup> _factory;
        private readonly HttpClient _client;

        public AdminModuleCoherencyTests(CustomWebApplicationFactory<P3AddNewFunctionalityDotNetCore.Startup> factory)
        {
            _factory = factory;
            _client = factory.CreateClient();
        }

        // Coherency check for product deletion from admin / client on the home page
        [Fact]
        public async Task ClientHomePageUpdatesProperlyAfterProductDeletion()
        {
            // Setup admin
            var adminClient = _factory.CreateClient();

            // Setup client
            var userClient = _factory.CreateClient();

            // client view page
            var clientResponse = await userClient.GetAsync("/Product");
            clientResponse.EnsureSuccessStatusCode();
            var originalClientPageData = await clientResponse.Content.ReadAsStringAsync();

            // admin view page

            var formData = new Dictionary<string, string>
            {
                { "Id", "4" },

            };
            var adminDeleteResponse = await adminClient.PostWithAuthAsync("/Product/ConfirmDelete?id=4", "/Product/DeleteProduct", formData);
            adminDeleteResponse.EnsureSuccessStatusCode();


            var clientReload = await userClient.GetAsync("/Product");
            clientReload.EnsureSuccessStatusCode();
            var newClientPageData = await clientReload.Content.ReadAsStringAsync();

            Assert.Contains("VTech CS6114 DECT 6.0", originalClientPageData);
            Assert.DoesNotContain("VTech CS6114 DECT 6.0", newClientPageData);
        }

        [Fact]
        public async Task ClientCartUpdatesProperlyAfterProductDeletion()
        {
            // Setup admin
            var adminClient = _factory.CreateClient();

            // Setup client
            var userClient = _factory.CreateClient();

            // client view page
            var productData = new Dictionary<string, string>
            {
                { "Id", "5" },

            };
            var clientResponse = await userClient.PostAsync("/Cart/AddToCart", new FormUrlEncodedContent(productData));
            clientResponse.EnsureSuccessStatusCode();
            var originalClientPageData = await clientResponse.Content.ReadAsStringAsync();

            // admin view page

            var formData = new Dictionary<string, string>
            {
                { "Id", "5" },

            };
            var adminDeleteResponse = await adminClient.PostWithAuthAsync("/Product/ConfirmDelete?id=4", "/Product/DeleteProduct", formData);
            adminDeleteResponse.EnsureSuccessStatusCode();


            var clientReload = await userClient.GetAsync("/Cart");
            clientReload.EnsureSuccessStatusCode();
            var newClientPageData = await clientReload.Content.ReadAsStringAsync();

            Assert.Contains("NOKIA OEM BL-5J", originalClientPageData);
            Assert.DoesNotContain("NOKIA OEM BL-5J", newClientPageData);
        }
    }
}
