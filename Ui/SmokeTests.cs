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

    [Test]
    public async Task ExecuteOnlyWithChromium()
    {
        Assume.That(BrowserName, Is.Not.EqualTo("firefox"),
            "Firefox shows a download prompt in this setup; covered by manual check.");

        // ... chromium-only steps
        TestContext.Progress.WriteLine(
            $"[{BrowserName}] {TestContext.CurrentContext.Test.Name} " +
            $"worker={TestContext.CurrentContext.WorkerId} thread={Environment.CurrentManagedThreadId}");
    }
}
