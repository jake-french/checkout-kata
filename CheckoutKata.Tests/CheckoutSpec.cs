using CheckoutKata.Core;
using CheckoutKata.Core.Models;

namespace CheckoutKata.Tests;

public class CheckoutSpec
{
    [Fact]
    public void GetCorrectPrice_When_NoItemsScanned()
    {
        var checkout = new Checkout([]);
        var result = checkout.GetTotalPrice();
        Assert.Equal(0, result);
    }
    
    [Fact]
    public void GetCorrectPrice_When_SingleItemScanned()
    {
        var checkout = new Checkout([new Product("A", 50)]);
        checkout.Scan("A");
        
        var result = checkout.GetTotalPrice();
        
        Assert.Equal(50, result);
    }
    
    [Fact]
    public void GetCorrectPrice_When_MultipleItemsScanned()
    {
        var checkout = new Checkout([new Product("A", 50), new Product("B", 30)]);
        checkout.Scan("A");
        checkout.Scan("B");
        
        var result = checkout.GetTotalPrice();
        
        Assert.Equal(80, result);
    }
}