using Microsoft.Extensions.Localization;
using Moq;
using P3AddNewFunctionalityDotNetCore.Models;
using P3AddNewFunctionalityDotNetCore.Models.Entities;
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
        public void Test_CheckProductModelErrors_ValidateMissingName()
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
        public void Test_CheckProductModelErrors_ValidateMissingPrice()
        {
            // Arrange
            
            // Configure Localization
            var localizedStringMissingPrice = new LocalizedString("MissingPrice", "Please enter a price");
            localizer.Setup(x => x["MissingPrice"]).Returns(localizedStringMissingPrice);

            var productService = new ProductService(_fixture.Cart.Object, _fixture.ProductRepo.Object,
                            _fixture.OrderRepo.Object, localizer.Object);

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
        public void Test_CheckProductModelErrors_ValidatePriceNotANumber()
        {
            // Arrange
             // Configure Localization
            var localizedString = new LocalizedString("PriceNotANumber", "The value entered for the price must be a number");
            localizer.Setup(x => x["PriceNotANumber"]).Returns(localizedString);

            var productService = new ProductService(_fixture.Cart.Object, _fixture.ProductRepo.Object,
                _fixture.OrderRepo.Object, localizer.Object);
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
        public void Test_CheckProductModelErrors_ValidatePriceNotGreaterThanZero()
        {
            // Arrange
            
            // Configure Localization
            var localizedString = new LocalizedString("PriceNotGreaterThanZero", "The price must be greater than zero");
            localizer.Setup(x => x["PriceNotGreaterThanZero"]).Returns(localizedString);

            var productService = new ProductService(_fixture.Cart.Object, _fixture.ProductRepo.Object,
                 _fixture.OrderRepo.Object, localizer.Object);
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
        public void Test_CheckProductModelErrors_ValidateStockMissingQuantity()
        {
            // Arrange
           
            // Configure Localization
            var localizedStringMissingStock = new LocalizedString("MissingStock", "Please enter a stock value");
            localizer.Setup(x => x["MissingStock"]).Returns(localizedStringMissingStock);

            var productService = new ProductService(_fixture.Cart.Object, _fixture.ProductRepo.Object,
                _fixture.OrderRepo.Object, localizer.Object);
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
        public void Test_CheckProductModelErrors_ValidateStockNotAnInteger()
        {
            // Arrange
            
            // Configure Localization
            var localizedString = new LocalizedString("StockNotAnInteger", "The value entered for the stock must be a number");
            localizer.Setup(x => x["StockNotAnInteger"]).Returns(localizedString);
            var productService = new ProductService(_fixture.Cart.Object, _fixture.ProductRepo.Object,
                            _fixture.OrderRepo.Object, localizer.Object);
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
        public void Test_CheckProductModelErrors_ValidateStockNotGreaterThanZero()
        {
            // Arrange
           // Configure Localization
            var localizedString = new LocalizedString("StockNotGreaterThanZero", "The stock must greater than zero");
            localizer.Setup(x => x["StockNotGreaterThanZero"]).Returns(localizedString);
            var productService = new ProductService(_fixture.Cart.Object, _fixture.ProductRepo.Object,
                            _fixture.OrderRepo.Object, localizer.Object);

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

        [Fact]
        public void Test_CanGetAllProductsAsViewModel()
        {
            var productService = new ProductService(_fixture.Cart.Object, _fixture.ProductRepo.Object,
                _fixture.OrderRepo.Object, localizer.Object);

            var products = productService.GetAllProductsViewModel();

            Assert.NotEmpty(products);
            Assert.IsType<List<ProductViewModel>>(products);
        }

        [Fact]
        public void Test_CanGetAllProducts()
        {
            var productService = new ProductService(_fixture.Cart.Object, _fixture.ProductRepo.Object,
                _fixture.OrderRepo.Object, localizer.Object);

            var products = productService.GetAllProducts();

            Assert.NotEmpty(products);
            Assert.IsType<List<Product>>(products);
        }

        [Fact]
        public void Test_CanGetProductById()
        {
            var productService = new ProductService(_fixture.Cart.Object, _fixture.ProductRepo.Object,
                _fixture.OrderRepo.Object, localizer.Object);

            var product = productService.GetProductById(1);

            Assert.NotNull(product);
            Assert.IsType<Product>(product);
            Assert.Equal(1, product.Id);
        }

        [Fact]
        public void Test_CanGetProductByIdAsViewModel()
        {
            var productService = new ProductService(_fixture.Cart.Object, _fixture.ProductRepo.Object,
                _fixture.OrderRepo.Object, localizer.Object);

            var product = productService.GetProductByIdViewModel(1);

            Assert.NotNull(product);
            Assert.IsType<ProductViewModel>(product);
            Assert.Equal(1, product.Id);
        }

        [Fact]
        public void Test_CanSaveProductViewModelAsProduct()
        {
            var productService = new ProductService(_fixture.Cart.Object, _fixture.ProductRepo.Object,
                _fixture.OrderRepo.Object, localizer.Object);

            ProductViewModel productViewModel = new ProductViewModel
            {
                Name = "New product", Description = "New Product from view model", Details = "",
                Stock = "999", Price = "12.99"
            };

            productService.SaveProduct(productViewModel);

            var product = productService.GetProductById(0);

            Assert.NotNull(product);
            Assert.Equal(0, product.Id);
            
        }
    }
}