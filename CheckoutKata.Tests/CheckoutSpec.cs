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
    public void GetCorrectPrice_When_ItemScanned()
    {
        var checkout = new Checkout([new Product("A", 50)]);
        checkout.Scan("A");
        
        var result = checkout.GetTotalPrice();
        
        Assert.Equal(50, result);
    }
}