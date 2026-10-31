using Microsoft.Playwright;

namespace PlaywrightDotNetLab.Api;

public abstract class ApiTestBase
{
    private IPlaywright _playwright = null!;

    protected IAPIRequestContext Api { get; private set; } = null!;

    [OneTimeSetUp]
    public async Task CreateApiContextAsync()
    {
        _playwright = await Playwright.CreateAsync();
        Api = await _playwright.APIRequest.NewContextAsync(new()
        {
            BaseURL = TestSettings.HerokuApiUrl,
            ExtraHTTPHeaders = new Dictionary<string, string>
            {
                ["Accept"] = "application/json",
                ["Content-Type"] = "application/json"
            }
        });
    }

    [OneTimeTearDown]
    public async Task DisposeApiContextAsync()
    {
        await Api.DisposeAsync();
        _playwright.Dispose();
    }
}
