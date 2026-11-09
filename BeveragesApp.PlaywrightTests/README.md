# BeveragesApp Playwright test app

This solution includes a small browser test app that recreates the Swedish home, beverage, category, country, and remark content from BeveragesApp. MSTest starts the app on a free local port; Playwright checks page content, add flows, unsaved-item delete validation, all list/detail back-navigation buttons, and the exit confirmation on every page. Beverage, category, country, remark, back-navigation, exit-button, and unsaved-delete checks are kept in separate test classes.

This is a web test harness, because BeveragesApp itself is a native .NET MAUI app and Playwright cannot automate its native controls directly.

Run the tests with:

```powershell
dotnet test .\BeveragesApp.PlaywrightTests\BeveragesApp.PlaywrightTests.csproj
```

Install Playwright's Chromium browser once after building:

```powershell
dotnet build .\BeveragesApp.PlaywrightTests\BeveragesApp.PlaywrightTests.csproj
powershell .\BeveragesApp.PlaywrightTests\bin\Debug\net10.0\playwright.ps1 install chromium
```
