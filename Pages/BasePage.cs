using Microsoft.Playwright;
using NUnit.Framework;

namespace PlaywrightDotNetLab.Pages;

public abstract class BasePage
{
    protected IPage Page { get; }

    protected BasePage(IPage page) => Page = page;

    /// <summary>Path relative to the context BaseURL, for example "/" or "/cart.html".</summary>
    protected abstract string RelativePath { get; }

    public virtual async Task OpenAsync() => await Page.GotoAsync(RelativePath);

    public async Task<string> GetTitleAsync() => await Page.TitleAsync();

    protected static void Log(string step) => TestContext.Progress.WriteLine($"  > {step}");

    protected async Task ClickAsync(ILocator locator, string description)
    {
        Log($"Click {description}");
        await locator.ClickAsync();
    }

    protected async Task FillAsync(ILocator locator, string value, string description)
    {
        Log($"Fill {description}");
        await locator.FillAsync(value);
    }

    protected async Task WaitForUrlAsync(string globPattern)
        => await Page.WaitForURLAsync(globPattern);
}