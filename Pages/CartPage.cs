using Microsoft.Playwright;

namespace PlaywrightDotNetLab.Pages;

public sealed class CartPage : BasePage
{
    public CartPage(IPage page) : base(page) { }

    protected override string RelativePath => "/cart.html";

    public ILocator Items => Page.GetByTestId("inventory-item");
    public ILocator ItemNames => Page.GetByTestId("inventory-item-name");

    public async Task<CheckoutPage> StartCheckoutAsync()
    {
        await ClickAsync(Page.GetByTestId("checkout"), "Checkout button");
        return new CheckoutPage(Page);
    }
}