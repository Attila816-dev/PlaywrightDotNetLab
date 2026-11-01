using System.Text.RegularExpressions;
using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using NUnit.Framework.Interfaces;

namespace PlaywrightDotNetLab.Ui;

[TestFixture]
public class UiTestBase : PageTest
{
    public override BrowserNewContextOptions ContextOptions()
    {
        var options = base.ContextOptions();
        options.BaseURL = TestSettings.SauceAppUrl;
        // options.StorageStatePath = UseSavedLogin ? GlobalSetup.AuthStatePath : null;
        options.RecordVideoDir = TestSettings.VideoTempDir;
        options.RecordVideoSize = new() { Width = 1280, Height = 720 };
        return options;
    }

    [SetUp]
    public void Setup() =>
        Playwright.Selectors.SetTestIdAttribute("data-test");

    [TearDown]
    public async Task CollectArtifactsAsync()
    {
        var failed = TestContext.CurrentContext.Result.Outcome.Status == TestStatus.Failed;
        var folder = Path.Combine(TestSettings.ArtifactsDir, BrowserName);
        var baseName = SafeFileName(TestContext.CurrentContext.Test.Name);
        var video = Page.Video;

        if (failed)
        {
            Directory.CreateDirectory(folder);
            await TryAsync("screenshot", async () =>
            {
                var path = Path.Combine(folder, $"{baseName}.png");
                await Page.ScreenshotAsync(new() { Path = path, FullPage = true });
                TestContext.AddTestAttachment(path, "Screenshot at failure");
            });
        }

        // Stopping with a Path writes the trace zip; with no Path the recording is discarded.
        var tracePath = failed ? Path.Combine(folder, $"{baseName}.zip") : null;
        await TryAsync("trace", async () => await Context.Tracing.StopAsync(new() { Path = tracePath }));
        if (tracePath is not null && File.Exists(tracePath))
            TestContext.AddTestAttachment(tracePath, "Playwright trace");

        // The video file is only finalised when the context closes.
        await Context.CloseAsync();

        if (video is not null)
        {
            await TryAsync("video", async () =>
            {
                if (failed)
                {
                    var path = Path.Combine(folder, $"{baseName}.webm");
                    await video.SaveAsAsync(path);
                    TestContext.AddTestAttachment(path, "Video of the test");
                }
                await video.DeleteAsync();  // remove the temp copy either way
            });
        }
    }

    private static string SafeFileName(string name)
        => Regex.Replace(name, @"[^\w\-\.]+", "_").Trim('_');

    /// <summary>Artifact collection must never hide the real test failure, so errors here are logged, not thrown.</summary>
    private static async Task TryAsync(string what, Func<Task> action)
    {
        try
        {
            await action();
        }
        catch (PlaywrightException ex)
        {
            TestContext.Progress.WriteLine($"Could not save {what}: {ex.Message}");
        }
        catch (IOException ex)
        {
            TestContext.Progress.WriteLine($"Could not write {what}: {ex.Message}");
        }
    }
}