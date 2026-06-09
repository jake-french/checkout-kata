namespace CheckoutKata.Core.Models;

public record Product(string SKU, int Price, int? QuantityForOffer = null, int? OfferPrice = null)
{
    public int GetPriceFor(int quantity)
    {
        if (QuantityForOffer is null || OfferPrice is null)
            return quantity * Price;
        
        var offerCount = quantity / QuantityForOffer.Value;
        var remainder = quantity % QuantityForOffer.Value;
            
        return offerCount * OfferPrice.Value + remainder * Price;
    }
}