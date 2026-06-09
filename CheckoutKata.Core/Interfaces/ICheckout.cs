namespace CheckoutKata.Core.Interfaces;

public interface ICheckout
{
    bool TryScan(string sku);
    int GetTotalPrice();
}