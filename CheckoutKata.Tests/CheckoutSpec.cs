using CheckoutKata.Core;

namespace CheckoutKata.Tests;

public class CheckoutSpec
{
    [Fact]
    public void GetCorrectPrice_When_NoItemsScanned()
    {
        var checkout = new Checkout();
        var result = checkout.GetTotalPrice();
        Assert.Equal(0, result);
    }
}