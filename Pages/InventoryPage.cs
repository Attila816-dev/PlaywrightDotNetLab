using Microsoft.Playwright;

namespace PlaywrightDotNetLab.Pages;

public sealed class InventoryPage : BasePage
{
    public InventoryPage(IPage page) : base(page) { }

    protected override string RelativePath => "/inventory.html";

    public ILocator Title => Page.GetByTestId("title");
    public ILocator CartBadge => Page.GetByTestId("shopping-cart-badge");
    public ILocator ProductNames => Page.GetByTestId("inventory-item-name");
    public ILocator ProductPrices => Page.GetByTestId("inventory-item-price");

    /// <param name="slug">Product slug, e.g. "sauce-labs-backpack".</param>
    public async Task AddToCartAsync(string slug)
        => await ClickAsync(Page.GetByTestId($"add-to-cart-{slug}"), $"Add to cart: {slug}");

    public async Task RemoveFromCartAsync(string slug)
        => await ClickAsync(Page.GetByTestId($"remove-{slug}"), $"Remove: {slug}");

    /// <param name="value">One of: az, za, lohi, hilo.</param>
    public async Task SortByAsync(string value)
        => await Page.GetByTestId("product-sort-container").SelectOptionAsync(value);

    public async Task<CartPage> OpenCartAsync()
    {
        await ClickAsync(Page.GetByTestId("shopping-cart-link"), "Cart icon");
        return new CartPage(Page);
    }
}