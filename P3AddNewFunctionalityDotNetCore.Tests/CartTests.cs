
using Microsoft.Extensions.Localization;
using Moq;
using P3AddNewFunctionalityDotNetCore.Models;
using P3AddNewFunctionalityDotNetCore.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace P3AddNewFunctionalityDotNetCore.Tests
{
    /// <summary>
    /// The Cart test class
    /// </summary>
    public class CartTests
    {
        [Fact]
        public void Test_AddItemInCart()
        {
            Cart cart = new Cart();
            Product product1 = new Product { Id = 1, Price = 10.99, Name = "Test Product 1", Quantity = 5 };
            Product product2 = new Product { Id = 1, Price = 10.99, Name = "Test Product 1", Quantity = 5 };

            cart.AddItem(product1, 1);
            cart.AddItem(product2, 1);

            Assert.NotEmpty(cart.Lines);
            Assert.Single<CartLine>(cart.Lines);
            Assert.Equal(2, cart.Lines.First().Quantity);
        }

        [Fact]
        public void Test_CannotAddItemIfMoreThanCurrentStock()
        {
            // Arrange
            Cart cart = new Cart();
            Product product = new Product {Id = 1, Price = 10.00, Name = "Test Product 1", Quantity = 5};

            // Act
            cart.AddItem(product, 10);

            // Assert
            Assert.Empty(cart.Lines);

        }

        [Fact]
        public void Test_RemoveItemInCart()
        {
            Cart cart = new Cart();
            Product product1 = new Product { Id = 1, Price = 10.99, Name = "Test Product 1", Quantity = 5 };

            cart.AddItem(product1, 3);

            Assert.NotEmpty(cart.Lines);

            cart.RemoveLine(product1);

            Assert.Empty(cart.Lines);
        }

        [Fact]
        public void Test_GetAverageValue()
        {
            Cart cart = new Cart();
            Product product1 = new Product { Id = 1, Price = 10.99, Name = "Test Product 1", Quantity = 5 };
            Product product2 = new Product { Id = 2, Price = 20.99, Name = "Test Product 2", Quantity = 5 };

            cart.AddItem(product1, 4);
            cart.AddItem(product2, 1);
                        
            double averageValue = cart.GetAverageValue();
            double expectedValue = ((10.99 * 4) + 20.99) / 5;

            Assert.Equal(expectedValue, averageValue);
        }

        [Fact]
        public void Test_GetTotalValue()
        {
            Cart cart = new Cart();
            Product product1 = new Product { Id = 1, Price = 10.99, Name = "Test Product 1", Quantity = 5 };
            Product product2 = new Product { Id = 2, Price = 20.99, Name = "Test Product 2", Quantity = 5 };

            cart.AddItem(product1, 4); 
            cart.AddItem(product2, 1); 
            double totalValue = cart.GetTotalValue(); 
            double expectedValue = 10.99 * 4 + 20.99; //64.95

            Assert.Equal(expectedValue, totalValue);
        }

    }
}
