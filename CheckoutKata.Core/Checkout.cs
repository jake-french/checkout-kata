using CheckoutKata.Core.Interfaces;

namespace CheckoutKata.Core;

public class Checkout : ICheckout
{
    public void Scan(string sku)
    {
        throw new NotImplementedException();
    }

    public int GetTotalPrice()
    {
        return 0;
    }
}