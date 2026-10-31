using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using PlaywrightDotNetLab.Pages;

namespace PlaywrightDotNetLab.Ui;

// [SetUpFixture]
public class GlobalSetup
{
    public static string AuthStatePath { get; } =
        Path.Combine(AppContext.BaseDirectory, "playwright-auth", "standard-user-credentials.json");

    // [OneTimeSetUp]
    public async Task RunBeforeAllTestsAsync()
    {
        Directory.CreateDirectory(Path.GetDirectoryName(AuthStatePath)!);

        using var playwright = await Playwright.CreateAsync();
        playwright.Selectors.SetTestIdAttribute("data-test");

        await using var browser = await playwright.Chromium.LaunchAsync(new() { Headless = true });
        var context = await browser.NewContextAsync(new() { BaseURL = TestSettings.SauceAppUrl });
        var page = await context.NewPageAsync();
        var loginPage = new LoginPage(page);

        try
        {
            await CookieBanner.RegisterAsync(page);

            await page.GotoAsync("/");
            await loginPage.LoginAsync(TestSettings.StandardUser, TestSettings.Password);
            await page.WaitForURLAsync("**/inventory.html");

            await context.StorageStateAsync(new() { Path = AuthStatePath });
        }
        catch (PlaywrightException ex)
        {
            // Fail the whole run early with a clear message instead of 40 confusing test failures.
            throw new InvalidOperationException(
                $"Global login failed at {page.Url}. Is {TestSettings.SauceAppUrl} reachable?", ex);
        }
    }

    // [OneTimeTearDown]
    public void RunAfterAllTests()
    {
        if (File.Exists(AuthStatePath)) File.Delete(AuthStatePath);   // do not leave sessions on disk
    }
}