using Microsoft.Playwright;

namespace PlaywrightDotNetLab.Pages;

public sealed class CheckoutPage : BasePage
{
    public CheckoutPage(IPage page) : base(page) { }

    protected override string RelativePath => "/checkout-step-one.html";

    public ILocator ConfirmationHeader => Page.GetByTestId("complete-header");

    public async Task EnterCustomerDetailsAsync(string first, string last, string zip)
    {
        await FillAsync(Page.GetByTestId("firstName"), first, "first name");
        await FillAsync(Page.GetByTestId("lastName"), last, "last name");
        await FillAsync(Page.GetByTestId("postalCode"), zip, "postal code");
        await ClickAsync(Page.GetByTestId("continue"), "Continue");
    }

    public async Task FinishAsync()
        => await ClickAsync(Page.GetByTestId("finish"), "Finish");
}