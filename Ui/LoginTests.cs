using System.Text.RegularExpressions;
using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using PlaywrightDotNetLab.Pages;

namespace PlaywrightDotNetLab.Ui;

[TestFixture]
public class LoginTests : UiTestBase
{
    [Test]
    public async Task Login_WithValidCredentials_ShowsProductList()
    {
        var login = new LoginPage(Page);
        var inventory = new InventoryPage(Page);

        await login.OpenAsync();
        await login.LoginAsync(TestSettings.StandardUser, TestSettings.Password);

        await Expect(Page).ToHaveURLAsync(new Regex("/inventory\\.html"));
        await Expect(inventory.Title).ToHaveTextAsync("Products");
    }

    [TestCase("locked_out_user", "secret_sauce", "Epic sadface: Sorry, this user has been locked out.")]
    [TestCase("standard_user", "wrong_password", "Epic sadface: Username and password do not match any user in this service")]
    [TestCase("", "secret_sauce", "Epic sadface: Username is required")]
    public async Task Login_WithInvalidCredentials_ShowsError(string user, string password, string expectedError)
    {
        var login = new LoginPage(Page);

        await login.OpenAsync();
        await login.LoginAsync(user, password);

        await Expect(login.ErrorMessage).ToHaveTextAsync(expectedError);
        await Expect(Page).Not.ToHaveURLAsync(new Regex("/inventory\\.html"));
    }
}
