using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using PlaywrightDotNetLab.Pages;

namespace PlaywrightDotNetLab.Ui;

[TestFixture]
public class ShoppingTests : UiTestBase
{
    private InventoryPage _inventory = null!;

    [SetUp]
    public async Task LoginAsync()
    {
        Playwright.Selectors.SetTestIdAttribute("data-test");
        var login = new LoginPage(Page);
        _inventory = new InventoryPage(Page);
        await login.OpenAsync();
        await login.LoginAsync(TestSettings.StandardUser, TestSettings.Password);
    }

    [Test]
    public async Task Cart_AddingAndRemovingItems_UpdatesBadge()
    {
        await _inventory.AddToCartAsync("sauce-labs-backpack");
        await _inventory.AddToCartAsync("sauce-labs-bike-light");
        await Expect(_inventory.CartBadge).ToHaveTextAsync("2");

        await _inventory.RemoveFromCartAsync("sauce-labs-backpack");
        await Expect(_inventory.CartBadge).ToHaveTextAsync("1");

        await _inventory.RemoveFromCartAsync("sauce-labs-bike-light");
        await Expect(_inventory.CartBadge).ToBeHiddenAsync();
    }

    [Test]
    public async Task Checkout_WithOneItem_ShowsOrderConfirmation()
    {
        await _inventory.AddToCartAsync("sauce-labs-backpack");
        var cart = await _inventory.OpenCartAsync();
        await Expect(cart.ItemNames).ToHaveTextAsync(new[] { "Sauce Labs Backpack" });

        var checkout = await cart.StartCheckoutAsync();
        await checkout.EnterCustomerDetailsAsync("Ada", "Lovelace", "1011");
        await checkout.FinishAsync();

        await Expect(checkout.ConfirmationHeader).ToHaveTextAsync("Thank you for your order!");
    }

    [TestCase("az", false)]
    [TestCase("za", true)]
    public async Task Products_SortedByName_AreInExpectedOrder(string sortValue, bool descending)
    {
        await _inventory.SortByAsync(sortValue);

        var names = await _inventory.ProductNames.AllInnerTextsAsync();

        Assert.That(names, Has.Count.EqualTo(6));
        if (descending)
            Assert.That(names, Is.Ordered.Descending);
        else
            Assert.That(names, Is.Ordered);
    }
}