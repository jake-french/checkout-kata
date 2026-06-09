using CheckoutKata.Core;
using CheckoutKata.Core.Models;

namespace CheckoutKata.Tests;

public class CheckoutSpec
{
    private static readonly List<Product> _products = [
        new ("A", 50, 2, 90),
        new ("B", 30, 3, 60),
        new ("C", 20, 2, 30)
    ];
    
    [Fact]
    public void GetCorrectPrice_When_NoItemsScanned()
    {
        var checkout = new Checkout([]);

        var result = checkout.GetTotalPrice();

        Assert.Equal(0, result);
    }

    [Theory]
    [
        InlineData("A", 50),
        InlineData("B", 30)
    ]
    public void GetCorrectPrice_When_SingleItemScanned(string sku, int expectedPrice)
    {
        var checkout = new Checkout(_products);

        Assert.True(checkout.TryScan(sku));

        var result = checkout.GetTotalPrice();

        Assert.Equal(expectedPrice, result);
    }

    [Theory]
    [
        InlineData(80, "A", "B"),
        InlineData(50, "C", "B"),
        InlineData(80, "B", "C", "B")
    ]
    public void GetCorrectPrice_When_MultipleItemsScanned(int expectedPrice, params string[] skus)
    {
        var checkout = new Checkout(_products);

        foreach (var sku in skus)
            Assert.True(checkout.TryScan(sku));

        var result = checkout.GetTotalPrice();

        Assert.Equal(expectedPrice, result);
    }

    [Fact]
    public void GetCorrectPrice_When_ItemsScanned_With_ApplicableOffers()
    {
        var checkout = new Checkout(_products);

        Assert.True(checkout.TryScan("A"));
        Assert.True(checkout.TryScan("A"));

        var result = checkout.GetTotalPrice();

        Assert.Equal(90, result);
    }

    [Fact]
    public void GetCorrectPrice_When_ItemsScanned_With_ApplicableOffers_And_Overflow()
    {
        var checkout = new Checkout(_products);

        Assert.True(checkout.TryScan("A"));
        Assert.True(checkout.TryScan("A"));
        Assert.True(checkout.TryScan("A"));

        var result = checkout.GetTotalPrice();

        Assert.Equal(140, result);
    }

    [Fact]
    public void GetCorrectPrice_When_ItemsScanned_With_ApplicableOffers_MultipleTimes()
    {
        var checkout = new Checkout(_products);

        for (var i = 0; i < 6; i++) // Scan 6 times
            Assert.True(checkout.TryScan("A"));

        var result = checkout.GetTotalPrice();

        Assert.Equal(270, result);
    }

    [Fact]
    public void GetCorrectPrice_When_ItemsScanned_With_ApplicableOffers_AnyOrder()
    {
        var checkout = new Checkout(_products);

        Assert.True(checkout.TryScan("A"));
        Assert.True(checkout.TryScan("B"));
        Assert.True(checkout.TryScan("A"));
        Assert.True(checkout.TryScan("B"));
        Assert.True(checkout.TryScan("B"));
        Assert.True(checkout.TryScan("A"));

        var result = checkout.GetTotalPrice();

        Assert.Equal(200, result);
    }

    [Fact]
    public void GetCorrectPrice_When_InvalidItemScanned()
    {
        var checkout = new Checkout(_products);
        Assert.False(checkout.TryScan("Z"));

        var result = checkout.GetTotalPrice();
        Assert.Equal(0, result);
    }

#pragma warning disable xUnit1012
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void GetCorrectPrice_When_InvalidStringGiven(string input)
    {
        var checkout = new Checkout(_products);
        Assert.False(checkout.TryScan(input));

        var result = checkout.GetTotalPrice();
        Assert.Equal(0, result);
    }
#pragma warning restore xUnit1012
}