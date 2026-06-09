namespace CheckoutKata.Core.Models;

public record Product(string SKU, int Price, int? QuantityForOffer = null, int? OfferPrice = null)
{
    
}