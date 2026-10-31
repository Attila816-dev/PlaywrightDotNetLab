using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;

namespace PlaywrightDotNetLab.Ui;

[TestFixture]
public class SmokeTests : PageTest
{
    [Test]
    public async Task SauceDemo_HomePage_HasExpectedTitle()
    {
        await Page.GotoAsync("https://www.saucedemo.com");

        await Expect(Page).ToHaveTitleAsync("Swag Labs");
        await Expect(Page.GetByPlaceholder("Username")).ToBeVisibleAsync();
    }
}
