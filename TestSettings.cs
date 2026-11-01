namespace PlaywrightDotNetLab;

public static class TestSettings
{
    public const string SauceAppUrl = "https://www.saucedemo.com";
    public const string HerokuAppUrl = "https://the-internet.herokuapp.com";
    public const string HerokuApiUrl = "https://restful-booker.herokuapp.com";
    public const string StandardUser = "standard_user";
    public const string Password = "secret_sauce";

    public static string ArtifactsDir { get; } =
        Path.Combine(AppContext.BaseDirectory, "TestResults", "artifacts");

    public static string VideoTempDir { get; } =
        Path.Combine(Path.GetTempPath(), "pw-videos");

    public static bool Headed => Environment.GetEnvironmentVariable("HEADED") == "1";
    public static float DefaultTimeoutMs => 10_000;
}