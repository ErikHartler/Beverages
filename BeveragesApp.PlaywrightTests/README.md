# BeveragesApp Playwright test app

This solution includes a small browser test app that recreates the Swedish home and beverage-list content from BeveragesApp. MSTest starts the app on a free local port; Playwright checks the home content, opens the beverage list, and adds a beverage.

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
