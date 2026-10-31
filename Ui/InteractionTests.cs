using System.Text.RegularExpressions;
using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using PlaywrightDotNetLab.Pages;

namespace PlaywrightDotNetLab.Ui;

[TestFixture]
public class InteractionTests : PageTest
{
    public override BrowserNewContextOptions ContextOptions()
    {
        var options = base.ContextOptions();
        options.BaseURL = TestSettings.HerokuAppUrl;
        return options;
    }

    [Test]
    public async Task Hover_OverFirstUser_RevealsCaptionAndProfileLink()
    {
        await Page.GotoAsync("/hovers");
        var firstUser = Page.Locator(".figure").First;
        var caption = firstUser.Locator(".figcaption");

        await Expect(caption).ToBeHiddenAsync();          // hidden before hover

        await firstUser.HoverAsync();

        await Expect(caption).ToBeVisibleAsync();         // visible after hover
        await Expect(caption).ToContainTextAsync("name: user1");

        await caption.GetByRole(AriaRole.Link, new() { Name = "View profile" }).ClickAsync();
        await Expect(Page).ToHaveURLAsync(new Regex("/users/1$"));
    }

    [Test]
    public async Task Typing_WithTypeAsync_ReportsLastKey()
    {
        await Page.GotoAsync("/key_presses");

        await Page.TypeAsync("#target", "Hello", new() { Delay = 50 }); // TypeAsync is deprecated in favour of FillAsync / PressSequentiallyAsync

        await Expect(Page.Locator("#result")).ToHaveTextAsync("You entered: O");
    }

    [Test]
    public async Task Typing_WithPressSequentially_ReportsLastKey()
    {
        await Page.GotoAsync("/key_presses");

        await Page.Locator("#target").PressSequentiallyAsync("Hello", new() { Delay = 50 });

        await Expect(Page.Locator("#target")).ToHaveValueAsync("Hello");
        await Expect(Page.Locator("#result")).ToHaveTextAsync("You entered: O");
    }
}