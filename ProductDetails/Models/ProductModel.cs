using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProductDetails.Models
{
    public class ProductModel
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public double ProductPrice { get; set; }
        public int Quantity { get; set; }
    }
}