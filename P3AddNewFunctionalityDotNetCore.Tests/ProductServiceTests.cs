using Microsoft.Extensions.Localization;
using Moq;
using P3AddNewFunctionalityDotNetCore.Models;
using P3AddNewFunctionalityDotNetCore.Models.Repositories;
using P3AddNewFunctionalityDotNetCore.Models.Services;
using P3AddNewFunctionalityDotNetCore.Models.ViewModels;
using System.Collections.Generic;
using Xunit;

namespace P3AddNewFunctionalityDotNetCore.Tests
{
    public class ProductServiceTests : IClassFixture<TestFixture>
    {
        private readonly TestFixture _fixture;
        private readonly Mock<IStringLocalizer<ProductService>> localizer;

        public ProductServiceTests(TestFixture fixture)
        {
            _fixture = fixture;
            localizer = new Mock<IStringLocalizer<ProductService>>();
        }

        // Test validation for missing product name
        [Fact]
        public void ValidateMissingName_CheckProductModelErrors()
        {
            // Arrange
            
            // Configure Localization
            var localizedString = new LocalizedString("MissingName", "Please enter your name");
            localizer.Setup(x => x["MissingName"]).Returns(localizedString);

            var productService = new ProductService(_fixture.Cart.Object, _fixture.ProductRepo.Object, 
                _fixture.OrderRepo.Object, localizer.Object);

            //Act
            var product = new ProductViewModel
            {
                Name = "",
                Price = "5",
                Description = "Test description",
                Details = "Product details",
                Stock = "1"
            };

            var modelErrors = productService.CheckProductModelErrors(product);

            Assert.Contains("Please enter your name", modelErrors);
        }

        // Test validation for missing price
        [Fact]
        public void ValidateMissingPrice_CheckProductModelErrors()
        {
            // Arrange
            Mock<ICart> cart = new Mock<ICart>();
            Mock<IProductRepository> pRepo = new Mock<IProductRepository>();
            Mock<IOrderRepository> oRepo = new Mock<IOrderRepository>();
            Mock<IStringLocalizer<ProductService>> localizer = new Mock<IStringLocalizer<ProductService>>();

            // Configure Localization
            var localizedStringMissingPrice = new LocalizedString("MissingPrice", "Please enter a price");
            localizer.Setup(x => x["MissingPrice"]).Returns(localizedStringMissingPrice);
            

            var productService = new ProductService(cart.Object, pRepo.Object, oRepo.Object, localizer.Object);

            //Act
            var product = new ProductViewModel
            {
                Name = "Test Product",
                Price = "",
                Description = "Test description",
                Details = "Product details",
                Stock = "1"
            };

            var modelErrors = productService.CheckProductModelErrors(product);


            Assert.Contains("Please enter a price", modelErrors);
        }

        // Test validation for price not a number
        [Fact]
        public void ValidatePriceNotANumber_CheckProductModelErrors()
        {
            // Arrange
            Mock<ICart> cart = new Mock<ICart>();
            Mock<IProductRepository> pRepo = new Mock<IProductRepository>();
            Mock<IOrderRepository> oRepo = new Mock<IOrderRepository>();
            Mock<IStringLocalizer<ProductService>> localizer = new Mock<IStringLocalizer<ProductService>>();

            // Configure Localization
            var localizedString = new LocalizedString("PriceNotANumber", "The value entered for the price must be a number");
            localizer.Setup(x => x["PriceNotANumber"]).Returns(localizedString);

            var productService = new ProductService(cart.Object, pRepo.Object, oRepo.Object, localizer.Object);

            //Act
            var product = new ProductViewModel
            {
                Name = "Test Product",
                Price = "Hello World",
                Description = "Test description",
                Details = "Product details",
                Stock = "1"
            };

            var modelErrors = productService.CheckProductModelErrors(product);


            Assert.Contains("The value entered for the price must be a number", modelErrors);
        }

        // Test validation for price not greater than zero
        [Fact]
        public void ValidatePriceNotGreaterThanZero_CheckProductModelErrors()
        {
            // Arrange
            Mock<ICart> cart = new Mock<ICart>();
            Mock<IProductRepository> pRepo = new Mock<IProductRepository>();
            Mock<IOrderRepository> oRepo = new Mock<IOrderRepository>();
            Mock<IStringLocalizer<ProductService>> localizer = new Mock<IStringLocalizer<ProductService>>();

            // Configure Localization
            var localizedString = new LocalizedString("PriceNotGreaterThanZero", "The price must be greater than zero");
            localizer.Setup(x => x["PriceNotGreaterThanZero"]).Returns(localizedString);

            var productService = new ProductService(cart.Object, pRepo.Object, oRepo.Object, localizer.Object);

            //Act
            var product = new ProductViewModel
            {
                Name = "Test Product",
                Price = "-9",
                Description = "Test description",
                Details = "Product details",
                Stock = "1"
            };

            var modelErrors = productService.CheckProductModelErrors(product);


            Assert.Contains("The price must be greater than zero", modelErrors);
        }

        // Test validation for stock missing quantity
        [Fact]
        public void ValidateStockMissingQuantity_CheckProductModelErrors()
        {
            // Arrange
            Mock<ICart> cart = new Mock<ICart>();
            Mock<IProductRepository> pRepo = new Mock<IProductRepository>();
            Mock<IOrderRepository> oRepo = new Mock<IOrderRepository>();
            Mock<IStringLocalizer<ProductService>> localizer = new Mock<IStringLocalizer<ProductService>>();

            // Configure Localization
            var localizedStringMissingStock = new LocalizedString("MissingStock", "Please enter a stock value");
            localizer.Setup(x => x["MissingStock"]).Returns(localizedStringMissingStock);
            

            var productService = new ProductService(cart.Object, pRepo.Object, oRepo.Object, localizer.Object);

            //Act
            var product = new ProductViewModel
            {
                Name = "Test Product",
                Price = "5",
                Description = "Test description",
                Details = "Product details",
                Stock = ""
            };

            var modelErrors = productService.CheckProductModelErrors(product);


            Assert.Contains("Please enter a stock value", modelErrors);
        }

        // Test validation for stock not an integer
        [Fact]
        public void ValidateStockNotAnInteger_CheckProductModelErrors()
        {
            // Arrange
            Mock<ICart> cart = new Mock<ICart>();
            Mock<IProductRepository> pRepo = new Mock<IProductRepository>();
            Mock<IOrderRepository> oRepo = new Mock<IOrderRepository>();
            Mock<IStringLocalizer<ProductService>> localizer = new Mock<IStringLocalizer<ProductService>>();

            // Configure Localization
            var localizedString = new LocalizedString("StockNotAnInteger", "The value entered for the stock must be a number");
            localizer.Setup(x => x["StockNotAnInteger"]).Returns(localizedString);

            var productService = new ProductService(cart.Object, pRepo.Object, oRepo.Object, localizer.Object);

            //Act
            var product = new ProductViewModel
            {
                Name = "Test Product",
                Price = "5",
                Description = "Test description",
                Details = "Product details",
                Stock = "Not an integer"
            };

            var modelErrors = productService.CheckProductModelErrors(product);


            Assert.Contains("The value entered for the stock must be a number", modelErrors);
        }

        // Test validation for stock not greater than zero
        [Fact]
        public void ValidateStockNotGreaterThanZero_CheckProductModelErrors()
        {
            // Arrange
            Mock<ICart> cart = new Mock<ICart>();
            Mock<IProductRepository> pRepo = new Mock<IProductRepository>();
            Mock<IOrderRepository> oRepo = new Mock<IOrderRepository>();
            Mock<IStringLocalizer<ProductService>> localizer = new Mock<IStringLocalizer<ProductService>>();

            // Configure Localization
            var localizedString = new LocalizedString("StockNotGreaterThanZero", "The stock must greater than zero");
            localizer.Setup(x => x["StockNotGreaterThanZero"]).Returns(localizedString);

            var productService = new ProductService(cart.Object, pRepo.Object, oRepo.Object, localizer.Object);

            //Act
            var product = new ProductViewModel
            {
                Name = "Test Product",
                Price = "5",
                Description = "Test description",
                Details = "Product details",
                Stock = "0"
            };

            var modelErrors = productService.CheckProductModelErrors(product);


            Assert.Contains("The stock must greater than zero", modelErrors);
        }
    }
}