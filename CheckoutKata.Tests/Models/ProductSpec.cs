using CheckoutKata.Core.Models;

namespace CheckoutKata.Tests.Models;

public class ProductSpec
{
    [Fact]
    public void BasePrice_CalculatedCorrectly()
    {
        var product = new Product("A", 50, 2, 90);
        var price = product.GetPriceFor(1);
        
        Assert.Equal(50, price);
    }
    
    [Fact]
    public void DiscountedPrice_CalculatedCorrectly()
    {
        var product = new Product("A", 50, 2, 90);
        var price = product.GetPriceFor(2);
        
        Assert.Equal(90, price);
    }
    
    [Fact]
    public void DiscountedPrice_CalculatedCorrectly_With_Overflow()
    {
        var product = new Product("A", 50, 2, 90);
        var price = product.GetPriceFor(3);
        
        Assert.Equal(140, price);
    }
}