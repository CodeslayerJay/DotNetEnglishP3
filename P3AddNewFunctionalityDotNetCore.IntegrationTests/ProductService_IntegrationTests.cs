using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using P3AddNewFunctionalityDotNetCore.Data;
using P3AddNewFunctionalityDotNetCore.Models;
using P3AddNewFunctionalityDotNetCore.Models.Entities;
using P3AddNewFunctionalityDotNetCore.Models.Repositories;
using P3AddNewFunctionalityDotNetCore.Models.Services;
using P3AddNewFunctionalityDotNetCore.Models.ViewModels;
using Xunit;

namespace P3AddNewFunctionalityDotNetCore.IntegrationTests
{
    public class ProductService_IntegrationTests
    {
        IStringLocalizer<ProductService> _localizer;

        [Fact]
        public void Test_CanGetAllProductsAsViewModel()
        {
            var cart = new Cart();
            var options = new DbContextOptionsBuilder<P3Referential>()
                //.UseInMemoryDatabase(databaseName: "Test_Database")
                .UseSqlServer("Server=(localdb)\\MSSQLLocalDB;Database=P3Referential-2f561d3b-493f-46fd-83c9-6e2643e7bd0a;Trusted_Connection=True;MultipleActiveResultSets=true")
                .Options;
            ProductService productService = null;
            var products = new List<ProductViewModel>();

            using (var ctx = new P3Referential(options))
            {
               productService = new ProductService(cart, 
                   new ProductRepository(ctx),
                   new OrderRepository(ctx), _localizer);
               products = productService.GetAllProductsViewModel();
            }
            
            Assert.NotEmpty(products);
            Assert.IsType<List<ProductViewModel>>(products);
        }

        [Fact]
        public void Test_CanGetAllProducts()
        {
            var cart = new Cart();
            var options = new DbContextOptionsBuilder<P3Referential>()
                //.UseInMemoryDatabase(databaseName: "Test_Database")
                .UseSqlServer("Server=(localdb)\\MSSQLLocalDB;Database=P3Referential-2f561d3b-493f-46fd-83c9-6e2643e7bd0a;Trusted_Connection=True;MultipleActiveResultSets=true")
                .Options;
            ProductService productService = null;
            var products = new List<Product>();

            using (var ctx = new P3Referential(options))
            {
                productService = new ProductService(cart,
                    new ProductRepository(ctx),
                    new OrderRepository(ctx), _localizer);
                products = productService.GetAllProducts();
            }

            Assert.NotEmpty(products);
            Assert.IsType<List<Product>>(products);
        }

        [Fact]
        public void Test_CanGetProductById()
        {
            var cart = new Cart();
            var options = new DbContextOptionsBuilder<P3Referential>()
                //.UseInMemoryDatabase(databaseName: "Test_Database")
                .UseSqlServer("Server=(localdb)\\MSSQLLocalDB;Database=P3Referential-2f561d3b-493f-46fd-83c9-6e2643e7bd0a;Trusted_Connection=True;MultipleActiveResultSets=true")
                .Options;
            ProductService productService = null;
            Product product = null;

            using (var ctx = new P3Referential(options))
            {
                productService = new ProductService(cart,
                    new ProductRepository(ctx),
                    new OrderRepository(ctx), _localizer);
                product = productService.GetProductById(1);
            }

            Assert.NotNull(product);
            Assert.Equal(1, product.Id);
        }

        [Fact]
        public void Test_CanGetProductByIdAsViewModel()
        {
            var cart = new Cart();
            var options = new DbContextOptionsBuilder<P3Referential>()
                //.UseInMemoryDatabase(databaseName: "Test_Database")
                .UseSqlServer("Server=(localdb)\\MSSQLLocalDB;Database=P3Referential-2f561d3b-493f-46fd-83c9-6e2643e7bd0a;Trusted_Connection=True;MultipleActiveResultSets=true")
                .Options;
            ProductService productService = null;
            ProductViewModel product = null;

            using (var ctx = new P3Referential(options))
            {
                productService = new ProductService(cart,
                    new ProductRepository(ctx),
                    new OrderRepository(ctx), _localizer);
                product = productService.GetProductByIdViewModel(1);
            }

            Assert.NotNull(product);
            Assert.Equal(1, product.Id);
            Assert.IsType<ProductViewModel>(product);
        }

        [Fact]
        public void Test_CanSaveNewProductToDb()
        {
            var cart = new Cart();
            var options = new DbContextOptionsBuilder<P3Referential>()
                .UseInMemoryDatabase(databaseName: "Test_Database")
                //.UseSqlServer("Server=(localdb)\\MSSQLLocalDB;Database=P3Referential-2f561d3b-493f-46fd-83c9-6e2643e7bd0a;Trusted_Connection=True;MultipleActiveResultSets=true")
                .Options;
            ProductService productService = null;
            ProductViewModel newProduct = new ProductViewModel
            {
                Name = "Test Product",
                Description = "This is a test product",
                Price = "5.99",
                Stock = "10",
                Details = ""
            };
            var product = new Product();

            using (var ctx = new P3Referential(options))
            {
                productService = new ProductService(cart,
                    new ProductRepository(ctx),
                    new OrderRepository(ctx), _localizer);
                productService.SaveProduct(newProduct);
                product = ctx.Product.FirstOrDefault(p => p.Name == "Test Product");
            }

            Assert.NotNull(product);
            
        }

        [Fact]
        public void Test_CanDeleteProductFromDbAndRemoveFromCart()
        {
            var cart = new Cart();
            var options = new DbContextOptionsBuilder<P3Referential>()
                .UseInMemoryDatabase(databaseName: "Test_Database")
                //.UseSqlServer("Server=(localdb)\\MSSQLLocalDB;Database=P3Referential-2f561d3b-493f-46fd-83c9-6e2643e7bd0a;Trusted_Connection=True;MultipleActiveResultSets=true")
                .Options;
            ProductService productService = null;
            ProductViewModel newProduct = new ProductViewModel
            {
                Name = "Test Product 2",
                Description = "This is a test product",
                Price = "5.99",
                Stock = "10",
                Details = ""
            };
            var product = new Product();

            using (var ctx = new P3Referential(options))
            {
                productService = new ProductService(cart,
                    new ProductRepository(ctx),
                    new OrderRepository(ctx), _localizer);
                productService.SaveProduct(newProduct);

                product = ctx.Product.FirstOrDefault(p => p.Name == "Test Product 2");
                Assert.NotNull(product);

                cart.AddItem(product, 3);
                Assert.NotEmpty(cart.Lines);

                productService.DeleteProduct(product.Id);
                product = productService.GetProductById(product.Id);

            }

            Assert.Null(product);
            Assert.Empty(cart.Lines);
        }

    }
}
