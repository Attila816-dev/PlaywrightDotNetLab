# PlaywrightDotNetLab

A .NET 10 NUnit project for browser UI and REST API testing with Microsoft Playwright.

## Requirements

- .NET 10 SDK
- PowerShell (`pwsh`) for the Playwright browser installer
- Microsoft Edge (the default browser channel in `playwright.runsettings`)

## Setup

Restore the project and install Edge for Playwright:

```bash
dotnet restore
pwsh bin/Debug/net10.0/playwright.ps1 install msedge
```

If you change the target framework or build configuration, adjust the installer path accordingly.

## Run tests

Run the full suite:

```bash
dotnet test
```

Run a specific test group:

```bash
dotnet test --filter FullyQualifiedName~LoginTests
dotnet test --filter FullyQualifiedName~BookingApiTests
```

The UI tests exercise SauceDemo and The Internet; API tests use Restful Booker. These tests require network access to the corresponding sites.

By default, Playwright launches Edge headlessly. To launch a headed browser, set `<Headless>false</Headless>` under `<Playwright><LaunchOptions>` in `playwright.runsettings`. A graphical display is required (on Linux without one, `xvfb-run -a dotnet test` provides a virtual display).

## Test report

Generate a Visual Studio TRX report under `TestResults/`:

```bash
dotnet test --logger "trx;LogFileName=playwright-test-results.trx"
```
