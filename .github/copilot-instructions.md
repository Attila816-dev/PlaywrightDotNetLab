# Copilot instructions for PlaywrightDotNetLab

## Project scope

This repository is a .NET 10 NUnit test project that covers browser UI flows with Microsoft Playwright and REST API checks against live public endpoints. The important mental model is: UI tests live under `Ui/`, API tests live under `Api/`, and shared page-object logic lives under `Pages/`.

## Build, test, and verification commands

Restore dependencies:

```bash
dotnet restore
```

Install the Playwright browser used by the project (Edge, as configured in `playwright.runsettings`):

```bash
pwsh bin/Debug/net10.0/playwright.ps1 install msedge
```

Run the full suite:

```bash
dotnet test
```

Run a specific test group or individual test with NUnit's `--filter`:

```bash
dotnet test --filter FullyQualifiedName~LoginTests
dotnet test --filter FullyQualifiedName~BookingApiTests
dotnet test --filter "Name~Login_WithValidCredentials_ShowsProductList"
```

Generate a TRX report in `TestResults/`:

```bash
dotnet test --logger "trx;LogFileName=playwright-test-results.trx"
```

This repo does not define a separate lint task; the project verification entry point is `dotnet test`.

## High-level architecture

- `Ui/` contains browser tests such as login, shopping, interaction, and smoke checks. These classes inherit from `UiTestBase`, which extends Microsoft Playwright's `PageTest` and sets a `data-test` selector attribute in `SetUp`.
- `Api/` contains API tests that create an `IAPIRequestContext` in `ApiTestBase` and hit the Restful Booker API via `BaseURL = TestSettings.HerokuApiUrl`.
- `Pages/` contains page objects (`LoginPage`, `InventoryPage`, `CartPage`, etc.). These classes inherit from `BasePage`, keep locators near the page they belong to, and expose task-oriented methods such as `LoginAsync()` and `OpenCartAsync()` instead of burying browser actions in the tests.
- `BrowserConfig.cs` centralizes browser launch settings and default timeouts. `TestSettings.cs` holds URLs, credentials, and environment-driven config like `HEADED`.
- `playwright.runsettings` is the repository default for browser selection and browser channel configuration (`chromium` + `msedge`).

## Key conventions in this codebase

- Prefer `GetByTestId(...)` selectors and keep the attribute as `data-test`. The project explicitly calls `Playwright.Selectors.SetTestIdAttribute("data-test")` in test setup.
- Keep tests readable and assertion-driven. UI tests often use `Expect(Page).ToHaveURLAsync(...)` and `Expect(locator).ToHaveTextAsync(...)` instead of custom polling logic.
- Do not duplicate browser setup across tests. Shared browser settings, URLs, and credentials live in `BrowserConfig.cs` and `TestSettings.cs`.
- Represent page flows as page objects, not ad hoc DOM manipulation in tests. A test should compose page object actions (`login.OpenAsync()`, `inventory.OpenCartAsync()`) and then assert on the state.
- The suite relies on live public test apps: SauceDemo for UI flows and Restful Booker for API flows. Treat these as integration tests against real endpoints rather than mocked services.
- For visual/headed execution, the code reads the `HEADED` environment variable (`HEADED=1`), and the README notes that Linux environments without a display may need `xvfb-run -a` before `dotnet test`.

## Working style for future changes

- Add or update tests alongside code changes when behavior is user-facing or API-contract-related.
- Keep app URLs, environment variables, and browser-launch behavior centralized in `TestSettings.cs` and `BrowserConfig.cs` rather than scattering them through tests.
- When adding new UI flows, follow the existing page-object pattern: new page class in `Pages/`, then a focused NUnit test in `Ui/` using the shared base classes.
