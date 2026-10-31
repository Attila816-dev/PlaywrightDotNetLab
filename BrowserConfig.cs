using Microsoft.Playwright;

namespace PlaywrightDotNetLab;

public static class BrowserConfig
{
    public static BrowserTypeLaunchOptions LaunchOptionsFor(string browserName) => browserName switch
    {
        "firefox" => new()
        {
            Headless = !TestSettings.Headed,
            FirefoxUserPrefs = new Dictionary<string, object>
            {
                ["intl.accept_languages"] = "en-US",
                ["network.cookie.cookieBehavior"] = 0   // accept all cookies
            }
        },
        "webkit" => new() { Headless = !TestSettings.Headed },
        _ => new()
        {
            Headless = !TestSettings.Headed,
            Args = new[] { "--disable-search-engine-choice-screen" }
        }
    };

    /// <summary>Firefox is often slower on first load; give it more room.</summary>
    public static float DefaultTimeoutFor(string browserName)
        => browserName == "firefox" ? 15_000 : TestSettings.DefaultTimeoutMs;
}