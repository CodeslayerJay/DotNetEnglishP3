using P3AddNewFunctionalityDotNetCore.Models.Entities;
using System.Collections.Generic;
using System.Linq;

namespace P3AddNewFunctionalityDotNetCore.Models
{
    public class Cart : ICart
    {
        private readonly List<CartLine> _cartLines;

        public Cart()
        {
            _cartLines = new List<CartLine>();
        }

        public void AddItem(Product product, int quantity)
        {
            CartLine line = _cartLines.FirstOrDefault(p => p.Product.Id == product.Id);

            if (line == null)
            {
                _cartLines.Add(new CartLine { Product = product, Quantity = quantity });
            }
            else
            {
                
                if (product.Quantity > line.Quantity)
                {
                    line.Quantity += quantity;
                }
            }
        }

        public void RemoveLine(Product product) => _cartLines.RemoveAll(l => l.Product.Id == product.Id);

        // These do not count if a line has a product with more
        // than 1 quantity to purchase
        // 
        //public double GetTotalValue()
        //{
        //    return _cartLines.Any() ? _cartLines.Sum(l => l.Product.Price) : 0;
        //}

        //public double GetAverageValue()
        //{
        //    return _cartLines.Any() ? _cartLines.Average(l => l.Product.Price) : 0;
        //}

        public double GetTotalValue()
        {
            double totalCartValue = 0.0;
            if (_cartLines.Any())
            {

                // Loop through each item in collection
                foreach (var item in _cartLines)
                {
                    // Multiply quantity by price then add to totalCartValue
                    totalCartValue = (item.Quantity * item.Product.Price) + totalCartValue;
                }

                //return totalCartValue;
            }

            // Return total value
            return totalCartValue;
        }

        public double GetAverageValue()
        {
            double totalCartValue = 0.0;
            if (_cartLines.Any())
            {
                int totalCartQuantity = 0;

                // Loop through each item in collection
                foreach (var item in _cartLines)
                {
                    // Add product quantity to total cart quantity
                    totalCartQuantity = totalCartQuantity + item.Quantity;

                    // Multiply quantity by price then add to totalCartValue
                    totalCartValue = (item.Quantity * item.Product.Price) + totalCartValue;
                }

                // Return average
                return totalCartValue / totalCartQuantity;

            }

            return totalCartValue;
        }

        public void Clear() => _cartLines.Clear();

        public IEnumerable<CartLine> Lines => _cartLines;
    }

    public class CartLine
    {
        public int OrderLineId { get; set; }
        public Product Product { get; set; }
        public int Quantity { get; set; }
    }
}
