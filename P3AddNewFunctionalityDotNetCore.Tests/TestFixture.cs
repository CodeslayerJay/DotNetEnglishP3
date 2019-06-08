using Microsoft.Extensions.Localization;
using Moq;
using P3AddNewFunctionalityDotNetCore.Models;
using P3AddNewFunctionalityDotNetCore.Models.Entities;
using P3AddNewFunctionalityDotNetCore.Models.Repositories;
using P3AddNewFunctionalityDotNetCore.Models.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P3AddNewFunctionalityDotNetCore.Tests
{
    public class TestFixture
    {
        public Mock<ICart> Cart { get; set; }
        public Mock<IProductRepository> ProductRepo { get; set; }
        public Mock<IOrderRepository> OrderRepo { get; set; }

        public TestFixture()
        {
            Cart = new Mock<ICart>();
            ProductRepo = new Mock<IProductRepository>();
            OrderRepo = new Mock<IOrderRepository>();

            SetupMockedProductRepository();
            //SetupMockedOrderRepository();


        }

        private void SetupMockedProductRepository()
        {
            List<Product> products = new List<Product>
            {
                new Product { Id = 1, Name = "Test Product 1", Price = 5.99, Quantity = 5,
                    Description = "Test product 1 description", Details = "" },
                new Product { Id = 2, Name = "Test Product 2", Price = 99.99, Quantity = 15,
                    Description = "Test product 2 description", Details = "" },
                new Product { Id = 3, Name = "Test Product 3", Price = 159.99, Quantity = 3,
                    Description = "Test product 3 description", Details = "" },
            };
            
            ProductRepo.Setup(x => x.GetAllProducts()).Returns(products);
            ProductRepo.Setup(x => x.GetProduct(It.IsAny<int>())).ReturnsAsync((int i) => products.SingleOrDefault(p => p.Id == i));

            ProductRepo.Setup(x => x.SaveProduct(It.IsAny<Product>())).Callback(
                (Product product) =>
                {
                    products.Add(product);
                });
            ProductRepo.Setup(x => x.DeleteProduct(It.IsAny<int>())).Callback((int i) =>
            {
                var product = products.First(p => p.Id == i);
                products.Remove(product);
            });
        }

        private void SetupMockedOrderRepository()
        {
            throw new NotImplementedException();
        }

    }
}
