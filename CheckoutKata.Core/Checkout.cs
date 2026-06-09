using CheckoutKata.Core.Interfaces;
using CheckoutKata.Core.Models;

namespace CheckoutKata.Core;

public class Checkout(IEnumerable<Product> products) : ICheckout
{
    private readonly Dictionary<string, Product> _productLookup = products.ToDictionary(p => p.SKU);
    private readonly Dictionary<string, int> _quantityLookup = new();
    
    public bool TryScan(string sku)
    {
        if (!_productLookup.ContainsKey(sku)) return false;
        
        if (!_quantityLookup.TryAdd(sku, 1))
            _quantityLookup[sku]++;

        return true;
    }

    public int GetTotalPrice() => _quantityLookup.Sum(kv =>
    {
        var product = _productLookup[kv.Key];
        if (product.QuantityForOffer is null || product.OfferPrice is null)
            return kv.Value * _productLookup[kv.Key].Price;
        
        var offerCount = kv.Value / product.QuantityForOffer.Value;
        var remainder = kv.Value % product.QuantityForOffer.Value;
            
        return offerCount * product.OfferPrice.Value + remainder * product.Price;
    });
}