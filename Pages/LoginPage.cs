using Microsoft.Playwright;

namespace PlaywrightDotNetLab.Pages;

public sealed class LoginPage : BasePage
{
    public LoginPage(IPage page) : base(page) { }

    protected override string RelativePath => "/";

    private ILocator UsernameInput => Page.GetByTestId("username");
    private ILocator PasswordInput => Page.GetByTestId("password");
    private ILocator LoginButton => Page.GetByTestId("login-button");

    public ILocator ErrorMessage => Page.GetByTestId("error");

    public async Task LoginAsync(string username, string password)
    {
        await FillAsync(UsernameInput, username, "username");
        await FillAsync(PasswordInput, password, "password");
        await ClickAsync(LoginButton, "Login button");
    }
}