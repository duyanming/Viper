using System;

namespace HelloWorldDto
{
    public class ProductDto
    {
        public string Name { get; set; }
        public int Number { get; set; }
        public double Price { get; set; }
        public double Amount { get { return Price * Number; } }
        public string CountryOfOrigin { get; set; }
    }
}
