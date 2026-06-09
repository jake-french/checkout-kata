namespace CheckoutKata.Core.Interfaces;

public interface ICheckout
{
    void Scan(string sku);
    int GetTotalPrice();
}