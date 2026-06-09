# Checkout Kata

## Scenario

### Synopsis

In a normal supermarket, things are identified using Stock Keeping Units, or SKUs. In our shop, we’ll use individual
letters of the alphabet (A, B, C, and so on). Our goods are priced individually. In addition, some items are
multi-priced: buy n of them, and they’ll cost you y pounds. For example, item ‘A’ might cost 50 pounds individually, but
this week we have a special offer: buy three ‘A’s and they’ll cost you 130. The current pricing and offers are as
follows:

| SKU | Unit Price | Special Price |
|-----|------------|---------------|
| A   | 50         | 3 for 130     |
| B   | 30         | 2 for 45      |
| C   | 20         |               |
| D   | 15         |               |

Our checkout accepts items in any order, so that if we scan a B, an A, and another B, we’ll recognize the two B’s and
price them at 45 (for a total price so far of 95). Because the pricing changes frequently, we need to be able to pass in
a set of pricing rules each time we start handling a checkout transaction.

### Instructions

Implement a class library that satisfies the problem described above. The solution should be test driven.

We're as interested in the process that you go through to develop the code as the end result, so commit early and often
so we can see the steps that you go through to arrive at your solution. We want to see a git repository containing your
solution, ideally uploaded to your own github account.

## Solution

### Building and Testing

This solution is implemented as a class library rather than a runnable application, so there is no console app or API
entry point to start directly. The test suite is the primary way to exercise and verify the kata implementation.

To build and test the solution, ensure the .NET SDK is installed. The project targets .NET 10, so a compatible .NET SDK
is required. If all prerequisites are met, run the following commands from the project root:

```bash
dotnet test
```

*Tests should force a build before executing. If not, run the following first:*

```bash
dotnet build
```

### Approach & Design

The solution was implemented using a test-driven development approach. I started with the smallest useful behaviour and
built the implementation incrementally around the tests.

The checkout is modelled around two main concepts:

- A `Product`, which owns the pricing rule for a SKU.
- A `Checkout`, which tracks scanned items and calculates the running total from the configured product rules.

This keeps pricing behaviour close to the product data, while allowing the checkout itself to remain focused on scanning
items and calculating totals.

The checkout receives its available products when it is created. This satisfies the requirement that pricing rules can
be provided each time a checkout transaction starts, rather than being hard-coded into the checkout implementation.

#### Decisions

- I used integer prices rather than decimals because the kata describes prices as whole-number values. This avoids
  unnecessary precision concerns and keeps the implementation simple.
- I modelled products as immutable records. A product represents a pricing rule for a SKU, so once created it should not
  need to change during an active checkout transaction.
- I eventually refactored offer calculation into the product model. This prevents the checkout from needing to
  understand the details of unit pricing versus multi-buy pricing.
- I used lookups internally for products and quantities. This makes scanning and total calculation straightforward:
  products are resolved by SKU, and scanned quantities are accumulated by SKU.
- I chose the `TryScan` pattern over custom exceptions to give developers explicit, compile-time visibility into both
  the success and failure paths. In a real-world checkout system, smudged or missing barcodes are expected operational
  events. Using exceptions for these common scenarios introduces fragile fault points that risk crashing the entire
  application. The `TryScan` method safely handles these expected failures without blocking the system or disrupting the
  user experience.

#### Assumptions

- The specific SKU prices and offers shown in the scenario are treated as example pricing data rather than fixed
  requirements. The implementation focuses on supporting configurable pricing rules supplied at the start of a checkout
  transaction.
- SKUs are case-sensitive.
- A blank, null, or whitespace SKU is invalid.
- Scanning an unknown SKU should fail safely and should not affect the current basket.
- Special offers are optional.
- A product either has a complete special offer, meaning both offer quantity and offer price are supplied, or no special
  offer applies.
- Prices are represented in the smallest relevant unit for the kata, using integers.
- The checkout only needs to calculate the total for the current transaction; persistence, receipts, tax, and payment
  are outside the scope of this kata.

### Implementation

The implementation was built around a simple public checkout contract:

- `TryScan(string sku)` attempts to scan a SKU and returns whether the scan was accepted.
- `GetTotalPrice()` calculates the total price of all accepted scans.

The checkout keeps two internal dictionaries:

- One dictionary maps SKU values to their configured products.
- One dictionary stores the quantity scanned for each SKU.

When a SKU is scanned, the checkout first validates that the input is not blank and that the SKU exists in the
configured product rules. If validation succeeds, the quantity for that SKU is incremented.

The total is calculated by asking each scanned product to price its current quantity. This means the checkout does not
need separate branching logic for discounted and non-discounted items.

The product pricing logic handles both simple unit pricing and multi-buy offers. For example, if a product has an offer
such as `3 for 130`, then a quantity of `4` is calculated as one offer group plus one remaining unit.

### Test-driven development

The implementation followed a red-green-refactor workflow:

1. Write a failing test for the next piece of behaviour.
2. Add the simplest production code needed to make the test pass.
3. Refactor once the behaviour is protected by tests.

The early tests focused on the smallest pricing behaviours first:

- A product with an offer still returns the normal unit price when the offer threshold has not been met.
- A product returns the special offer price when the required quantity is met.
- A product correctly combines offer pricing and normal unit pricing when the quantity exceeds the offer threshold.

Once the product pricing behaviour was covered, the checkout behaviour could be implemented on top of that lower-level
pricing rule.

This helped keep the checkout implementation small because the more detailed pricing rules were already tested in
isolation.

### Tests

The test suite covers both product-level pricing behaviour and checkout-level transaction behaviour.

Product pricing tests cover:

- Base price calculation.
- Discounted price calculation.
- Discounted price calculation with remaining non-discounted items.

Checkout tests cover:

- Returning `0` when no items have been scanned.
- Calculating the total for a single scanned item.
- Calculating the total for multiple different scanned items.
- Applying offers when the required quantity is scanned.
- Applying offers with remaining non-discounted items.
- Applying the same offer multiple times.
- Rejecting unknown SKUs.
- Rejecting invalid string input such as `null` or an empty string.
- Ensuring failed scans do not affect the total.

Together, these tests cover the main kata scenarios: configurable pricing rules, scanning items in a transaction,
applying multi-buy offers, and safely handling invalid scans.

The suite also uses parameterised tests where useful. This allows the same behaviour to be checked across multiple
inputs,
which adds confidence without duplicating test code.

### Future Improvements

If this kata were to evolve into a full production-ready system, the following are some areas I would dedicate time to
testing and implementing:

- Add a more extensible model for product offers. Instead of being hardcoded to handle quantity-based price reductions,
  use an abstraction around pricing rules to support scenarios such as:
    - Product group offers, for example, "any fruit 3 for 2".
    - Cross-product offers, for example, "buy A, get B free".
- Enforce product property validation for SKU (regex/fixed-format) and prices.
- Introduce a dedicated money value object for prices, including currency support if the checkout needed to handle
  multiple currencies.
- Provide reasons for failed scans in `TryScan` so callers can handle different failure cases explicitly.
- Provide bulk scan capabilities to improve performance and ergonomics for larger baskets.
- Introduce a product/pricing lookup data source with asynchronous access and caching, keeping retrieval concerns
  separate from checkout transaction logic.

