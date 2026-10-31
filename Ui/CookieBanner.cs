using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using PlaywrightDotNetLab.Pages;

namespace PlaywrightDotNetLab.Ui;

public static class CookieBanner
{
    // Adjust the accessible name to match the site you test, e.g. "Accept all", "I agree", "Consent".
    private static readonly string[] AcceptNames = { "Accept all", "Accept", "I agree", "Consent" };

    public static async Task RegisterAsync(IPage page)
    {
        foreach (var name in AcceptNames)
        {
            var acceptButton = page.GetByRole(AriaRole.Button, new() { Name = name, Exact = true });

            await page.AddLocatorHandlerAsync(acceptButton, async button =>
            {
                await button.ClickAsync();
            });
        }
    }

    public async static Task AcceptCookiesAsync(IBrowserContext browserContext, string url)
    {
        await browserContext.AddCookiesAsync(
        [
            new Cookie { Name = "cookie_consent", Value = "accepted", Url = url }
        ]);
    }
}