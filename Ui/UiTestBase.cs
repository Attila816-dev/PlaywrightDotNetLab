using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using PlaywrightDotNetLab.Pages;

namespace PlaywrightDotNetLab.Ui;

[TestFixture]
public class UiTestBase : PageTest
{
    public override BrowserNewContextOptions ContextOptions()
    {
        var options = base.ContextOptions();
        options.BaseURL = TestSettings.SauceAppUrl;
        return options;
    }   
    
    [SetUp]
    public void Setup() =>
        Playwright.Selectors.SetTestIdAttribute("data-test");
}