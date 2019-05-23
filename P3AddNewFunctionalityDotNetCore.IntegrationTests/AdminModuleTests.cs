using Microsoft.AspNetCore.Mvc.Testing;
using P3AddNewFunctionalityDotNetCore.IntegrationTests.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace P3AddNewFunctionalityDotNetCore.IntegrationTests
{
    public class AdminModuleTests : IClassFixture<WebApplicationFactory<P3AddNewFunctionalityDotNetCore.Startup>>
    {
        // Integration Tests of the Admin Module using HttpClient
        // *Integration Testing deals with the application as a whole (external resources, etc) to make
        // sure those part(s) of the application work together. Unit tests are used for the logic of the
        // individual pieces to determine they are working. (i.e specific controller tests)

        // Links and Resources used:
        // https://docs.microsoft.com/en-us/aspnet/core/test/integration-tests?view=aspnetcore-2.2#customize-webapplicationfactory
        // https://www.dotnetcurry.com/aspnet-core/1420/integration-testing-aspnet-core

        private readonly WebApplicationFactory<P3AddNewFunctionalityDotNetCore.Startup> _factory;

        public AdminModuleTests(WebApplicationFactory<P3AddNewFunctionalityDotNetCore.Startup> factory)
        {
            _factory = factory;
        }

        // Test can get login page
        [Fact]
        public async Task CanGetLoginPage()
        {
            var client = _factory.CreateClient();
            var url = "/Account/Login";

            var response = await client.GetAsync(url);

            response.EnsureSuccessStatusCode();
            Assert.Equal("/Account/Login", response.RequestMessage.RequestUri.AbsolutePath);
        }

        // Cannot access protected routes when not logged in as administrator
        [Theory]
        [InlineData("/Product/Admin")]
        [InlineData("/Product/Create")]
        [InlineData("/Product/ConfirmDelete?id=1")]
        public async Task CannotAccessProtectedResource_RedirectsToAccountLogin(string url)
        {
            var client = _factory.CreateClient();
            
            var response = await client.GetAsync(url);

            response.EnsureSuccessStatusCode();
            Assert.Equal("text/html; charset=utf-8", response.Content.Headers.ContentType.ToString());
            Assert.Equal("/Account/Login", response.RequestMessage.RequestUri.AbsolutePath);
        }

        // Test product creation (post)
        [Fact]
        public async Task CreateProductSuccessfully_RedirectsToAdminPage()
        {
            // Arrange 
            var client = _factory.CreateClient();
            var url = "/Product/Create";
            var loginDetails = new Dictionary<string, string> { { "Name", "Admin" }, { "Password", "P@ssword123" } };
            var formData = new Dictionary<string, string>
            {
                { "Name", "Integration Test Product" },
                { "Stock", "5" },
                { "Price", "5.00" },
                { "Description", "Integration test product" },
                { "Details", "" }
            };

            // Act
            // Login as administrator
            var loginResponse = await ClientHelpers.PostAntiForgeryAsync(client, "/Account/Login", loginDetails);
            loginResponse.EnsureSuccessStatusCode();

            // Make a post request
            var response = await ClientHelpers.PostAntiForgeryAsync(client, url, formData);
            response.EnsureSuccessStatusCode();

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal("/Product/Admin", response.RequestMessage.RequestUri.AbsolutePath);
        }

        // Can login successfully
        [Fact]
        public async Task CanLoginSuccessfully_RedirectsToAdmin()
        {
            // Arrange 
            var client = _factory.CreateClient();
            var url = "/Account/Login";

            var formData = new Dictionary<string, string>
            {
                { "Name", "Admin" },
                { "Password", "P@ssword123" },
                { "ReturnUrl", "/Product/Admin" }
            };

            // Act
            var response = await ClientHelpers.PostAntiForgeryAsync(client, url, formData);

            //var response = await client.PostAsync(url, new FormUrlEncodedContent(formData));
            response.EnsureSuccessStatusCode();

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal("/Product/Admin", response.RequestMessage.RequestUri.AbsolutePath);
            
        }
    }
}
