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
        //private readonly HttpClient _client;

        public AdminModuleTests(CustomWebApplicationFactory<P3AddNewFunctionalityDotNetCore.Startup> factory)
        {
            _factory = factory;
            //_client = factory.CreateClient();
            ClientHelpers.Stage(factory.CreateClient());
        }

        // Verify the user can access the login page
        [Fact]
        public async Task Get_CanGetLoginPage()
        {
            // Arrange
            var url = "/Account/Login";

            // Act
            var response = await ClientHelpers.GetAsync(url);
            response.EnsureSuccessStatusCode();

            // Assert
            Assert.Equal("/Account/Login", response.RequestMessage.RequestUri.AbsolutePath);
        }

        // Verify the user can logout as administrator
        [Fact]
        public async Task Get_CanLogoutAsAdmin_RedirectsToDefaultHomePage()
        {
            // Arrange
            var url = "/Account/Logout";

            // Act
            var response = await ClientHelpers.GetAsAuthAsync(url);
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
            
            var response = await ClientHelpers.GetAsync(url);

            response.EnsureSuccessStatusCode();
            Assert.Equal("/Account/Login", response.RequestMessage.RequestUri.AbsolutePath);
        }

        // Verify the user can get delete confirm page when logged in as administrator
        [Fact]
        public async Task Get_CanGetDeleteConfirmPageAsAdmin()
        {
            // Arrange
            var url = "/Product/ConfirmDelete?id=1";

            // Act
            var response = await ClientHelpers.GetAsAuthAsync(url);
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

        // Verify the admin page displays product information
        [Fact]
        public async Task Get_AdminPageShouldDisplayProductsFromDB()
        {
            // Arrange 
            var url = "/Product/Admin";
            
            // Act
            var response = await ClientHelpers.GetAsAuthAsync(url);
            response.EnsureSuccessStatusCode();

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal("/Product/Admin", response.RequestMessage.RequestUri.AbsolutePath);
            
            // Assert validation errors exist on page
            var responseString = response.Content.ReadAsStringAsync();
            
            Assert.Contains("<td>NOKIA OEM BL-5J</td>", responseString.Result);
            Assert.Contains("<td>VTech CS6114 DECT 6.0</td>", responseString.Result);

        }

        // Verify the user can create a new product when logged in as admin
        [Fact]
        public async Task Post_CreatesProductSuccessfully_RedirectsToAdminPage()
        {
            // Arrange 
            var url = "/Product/Create";
            var formData = new Dictionary<string, string>
            {
                { "Name", "Integration Test Product" },
                { "Stock", "5" },
                { "Price", "5.00" },
                { "Description", "Integration test product" },
                { "Details", "" }
            };

            // Act
            var response = await ClientHelpers.PostWithAuthAsync(url, formData);
            response.EnsureSuccessStatusCode();

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal("/Product/Admin", response.RequestMessage.RequestUri.AbsolutePath);

            // Verify product is available
            var responseString = response.Content.ReadAsStringAsync();

            Assert.Contains("<td>Integration Test Product</td>", responseString.Result);
        }

        // Verify product creation fails when inputing invalid data, returns with validation errors
        [Fact]
        public async Task Post_CreatesProductFailsWithValidationErrors()
        {
            // Arrange 
            
            var url = "/Product/Create";
            
            var formData = new Dictionary<string, string>
            {
                { "Name", "" },
                { "Stock", "" },
                { "Price", "" },
                { "Description", "Integration test product" },
                { "Details", "" }
            };

            // Act
            var response = await ClientHelpers.PostWithAuthAsync(url, formData);
            response.EnsureSuccessStatusCode();

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            

            // Assert validation errors exist on page
            var responseString = response.Content.ReadAsStringAsync();
            Assert.Contains("<div class=\"text-danger validation-summary-errors\" data-valmsg-summary=\"true\">", responseString.Result);
            
        }

        // Verify admin can successfully delete a product
        [Fact]
        public async Task Post_DeleteProductSuccessfully_RedirectsToAdminPage()
        {
            // Arrange 
            
            var url = "/Product/Create";
            
            var formData = new Dictionary<string, string>
            {
                { "Name", "Integration Test Product" },
                { "Stock", "5" },
                { "Price", "5.00" },
                { "Description", "Integration test product" },
                { "Details", "" }
            };

            // Act
            var response = await ClientHelpers.PostWithAuthAsync(url, formData);
            response.EnsureSuccessStatusCode();

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal("/Product/Admin", response.RequestMessage.RequestUri.AbsolutePath);
        }


    }
}
