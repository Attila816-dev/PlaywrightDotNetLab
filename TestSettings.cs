namespace PlaywrightDotNetLab;

public static class TestSettings
{
    public const string SauceAppUrl = "https://www.saucedemo.com";    
    public const string HerokuAppUrl = "https://the-internet.herokuapp.com";
    public const string StandardUser = "standard_user";
    public const string Password = "secret_sauce";

    public static bool Headed => Environment.GetEnvironmentVariable("HEADED") == "1";
    public static float DefaultTimeoutMs => 10_000;
}