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

            // Act for user client
            // Client user adds item to cart before its removed
            var clientResponse = await userClient.GetAsync("/Product");
            clientResponse.EnsureSuccessStatusCode();
            var originalClientPageData = await clientResponse.Content.ReadAsStringAsync();

            // Act for admin
            // Admin deletes the item
            var formData = new Dictionary<string, string>
            {
                { "Id", "4" },

            };
            var adminDeleteResponse = await adminClient.PostWithAuthAsync("/Product/ConfirmDelete?id=4", "/Product/DeleteProduct", formData);
            adminDeleteResponse.EnsureSuccessStatusCode();

            // Act -> Do updates to test for differences and assertions.
            // Reload the cart for user client
            var clientReload = await userClient.GetAsync("/Product");
            clientReload.EnsureSuccessStatusCode();
            // Extract the page data
            var newClientPageData = await clientReload.Content.ReadAsStringAsync();

            // Assert the orinigal user client product page and the updated product page do not match
            // (Product has been removed from the page successfully and updated)
            Assert.Contains("VTech CS6114 DECT 6.0", originalClientPageData);
            Assert.DoesNotContain("VTech CS6114 DECT 6.0", newClientPageData);
        }

        [Fact]
        public async Task ClientCartUpdatesProperlyAfterProductDeletion()
        {
            // Setup admin
            var adminClient = _factory.CreateClient();

            // Setup user client
            var userClient = _factory.CreateClient();

            // Act for user client
            // Client user adds item to cart before its removed
            var productData = new Dictionary<string, string>
            {
                { "Id", "3" },

            };
            var clientResponse = await userClient.PostAsync("/Cart/AddToCart", new FormUrlEncodedContent(productData));
            clientResponse.EnsureSuccessStatusCode();
            // Extract the page view & data
            var originalClientPageData = await clientResponse.Content.ReadAsStringAsync();


            // Act for admin
            // Admin deletes the item
            var formData = new Dictionary<string, string>
            {
                { "Id", "3" },

            };
            var adminDeleteResponse = await adminClient.PostWithAuthAsync("/Product/ConfirmDelete?id=3", "/Product/DeleteProduct", formData);
            adminDeleteResponse.EnsureSuccessStatusCode();

            // Act -> Do updates to test for differences and assertions.
            // Reload the cart for user client
            var clientReload = await userClient.GetAsync("/Cart");
            clientReload.EnsureSuccessStatusCode();
            // Extract the page view & data
            var newClientPageData = await clientReload.Content.ReadAsStringAsync();

            // Assert the orinigal user client cart and the updated cart do not match
            // (Product has been removed from cart successfully and updated)
            Assert.Contains("JVC HAFX8R Headphone", originalClientPageData);
            Assert.DoesNotContain("JVC HAFX8R Headphone", newClientPageData);
        }
    }
}
