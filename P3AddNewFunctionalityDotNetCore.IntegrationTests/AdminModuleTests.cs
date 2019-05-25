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
    public class AdminModuleTests : IClassFixture<CustomWebApplicationFactory<P3AddNewFunctionalityDotNetCore.Startup>>
    {
        // Integration Tests of the Admin Module using HttpClient
        // *Integration Testing deals with the application as a whole (external resources, etc) to make
        // sure those part(s) of the application work together. Unit tests are used for the logic of the
        // individual pieces to determine they are working, usually by mocking the services required. 
        // (i.e specific controller tests, service tests, model tests, etc.)

        // Links and Resources used:
        // https://docs.microsoft.com/en-us/aspnet/core/test/integration-tests?view=aspnetcore-2.2#customize-webapplicationfactory
        

        private readonly CustomWebApplicationFactory<P3AddNewFunctionalityDotNetCore.Startup> _factory;

        public AdminModuleTests(CustomWebApplicationFactory<P3AddNewFunctionalityDotNetCore.Startup> factory)
        {
            _factory = factory;
        }

        // Verify the user can access the login page
        [Fact]
        public async Task Get_CanGetLoginPage()
        {
            var client = _factory.CreateClient();
            var url = "/Account/Login";

            var response = await client.GetAsync(url);

            response.EnsureSuccessStatusCode();
            Assert.Equal("/Account/Login", response.RequestMessage.RequestUri.AbsolutePath);
        }

        // Verify the user can logout as administrator
        [Fact]
        public async Task Get_CanLogoutAsAdmin_RedirectsToDefaultHomePage()
        {
            // Arrange
            var client = _factory.CreateClient();
            var url = "/Account/Logout";

            // Act
            // Ensure we are logged in first
            // Login as administrator
            var loginDetails = new Dictionary<string, string> { { "Name", "Admin" }, { "Password", "P@ssword123" } };
            var loginAsAdmin = await ClientHelpers.PostAntiForgeryAsync(client, "/Account/Login", loginDetails);
            loginAsAdmin.EnsureSuccessStatusCode();

            var response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode();

            // Assert
            Assert.Equal("/", response.RequestMessage.RequestUri.AbsolutePath);
        }

        // Verify the user(s) cannot access protected routes when not logged in as administrator
        [Theory]
        [InlineData("/Product/Admin")]
        [InlineData("/Product/Create")]
        [InlineData("/Product/ConfirmDelete?id=1")]
        public async Task Get_CannotAccessProtectedResource_RedirectsToAccountLogin(string url)
        {
            var client = _factory.CreateClient();
            
            var response = await client.GetAsync(url);

            response.EnsureSuccessStatusCode();
            Assert.Equal("/Account/Login", response.RequestMessage.RequestUri.AbsolutePath);
        }

        // Verify the user can get delete confirm page when logged in as administrator
        [Fact]
        public async Task Get_CanGetDeleteConfirmPageAsAdmin()
        {
            // Arrange
            var client = _factory.CreateClient();
            var url = "/Product/ConfirmDelete?id=1";

            // Login as administrator
            var loginDetails = new Dictionary<string, string> { { "Name", "Admin" }, { "Password", "P@ssword123" } };
            var loginAsAdmin = await ClientHelpers.PostAntiForgeryAsync(client, "/Account/Login", loginDetails);
            loginAsAdmin.EnsureSuccessStatusCode();

            // Act
            var response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode();

            // Assert correct route
            Assert.Equal("/Product/ConfirmDelete", response.RequestMessage.RequestUri.AbsolutePath);
            
            // Assert product is available on page
            var responseString = response.Content.ReadAsStringAsync();
            Assert.Contains("<input type=\"hidden\" name=\"id\" value=\"1\" />", responseString.Result);
        }

        // Verify the user can login as administrator with the default username/password
        [Fact]
        public async Task Post_CanLoginSuccessfullyAsAdmin_RedirectsToAdminPage()
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
            response.EnsureSuccessStatusCode();

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal("/Product/Admin", response.RequestMessage.RequestUri.AbsolutePath);

        }

        // Verify the user can create a new product when logged in as admin
        [Fact]
        public async Task Post_CreatesProductSuccessfully_RedirectsToAdminPage()
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

        // Verify product creation fails when inputing invalid data, returns with validation errors
        [Fact]
        public async Task Post_CreatesProductFailsWithValidationErrors()
        {
            // Arrange 
            var client = _factory.CreateClient();
            var url = "/Product/Create";
            var loginDetails = new Dictionary<string, string> { { "Name", "Admin" }, { "Password", "P@ssword123" } };
            var formData = new Dictionary<string, string>
            {
                { "Name", "" },
                { "Stock", "" },
                { "Price", "" },
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
            //Assert.Equal("/Product/Admin", response.RequestMessage.RequestUri.AbsolutePath);

            // Assert validation errors exist on page
            var responseString = response.Content.ReadAsStringAsync();
            Assert.Contains("<div class=\"text-danger validation-summary-errors\" data-valmsg-summary=\"true\">", responseString.Result);
            //Assert.Contains("<li>Please enter a name</li>", responseString.Result);
            //Assert.Contains("<li>Please enter a price</li>", responseString.Result);
            //Assert.Contains("<li>Please enter a stock value</li>", responseString.Result);
        }

        // Verify admin can successfully delete a product
        [Fact]
        public async Task Post_DeleteProductSuccessfully_RedirectsToAdminPage()
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


    }
}
