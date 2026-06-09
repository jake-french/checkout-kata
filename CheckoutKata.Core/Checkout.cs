using CheckoutKata.Core.Interfaces;
using CheckoutKata.Core.Models;

namespace CheckoutKata.Core;

public class Checkout(IEnumerable<Product> products) : ICheckout
{
    private readonly Dictionary<string, Product> _productLookup = products.ToDictionary(p => p.SKU);
    private readonly Dictionary<string, int> _quantityLookup = new();
    
    public void Scan(string sku)
    {
        if (!_quantityLookup.TryAdd(sku, 1))
            _quantityLookup[sku]++;
    }

    public int GetTotalPrice()
    {
        return _quantityLookup.Sum(kv => kv.Value * _productLookup[kv.Key].Price);
    }
}