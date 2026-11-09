using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace BeveragesApp.PlaywrightTestApp;

public static class WebTestApp
{
    public static WebApplication Create(string[]? args = null)
    {
        var builder = WebApplication.CreateBuilder(args ?? []);
        var app = builder.Build();
        app.MapGet("/", () => Results.Content(Page, "text/html; charset=utf-8"));
        return app;
    }

    private const string Page = """
        <!doctype html>
        <html lang="sv">
        <head>
          <meta charset="utf-8">
          <meta name="viewport" content="width=device-width, initial-scale=1">
          <title>BeveragesApp</title>
          <style>
            :root { font-family: Segoe UI, sans-serif; color: #202124; background: #f7f5fc; }
            body { margin: 0; min-height: 100vh; display: grid; place-items: center; }
            main { box-sizing: border-box; width: min(760px, 100%); padding: 32px; background: white; border-radius: 16px; box-shadow: 0 8px 32px #24134b1a; }
            h1 { text-align: center; color: #33205f; }
            .actions { display: flex; justify-content: center; gap: 12px; margin: 24px 0; }
            .icon-button { width: 48px; height: 48px; display: inline-grid; place-items: center; border: 1px solid #ddd5f0; border-radius: 12px; background: #f8f6ff; color: #512bd4; cursor: pointer; }
            .icon-button:hover { background: #eee9ff; }
            .icon-button svg { width: 24px; height: 24px; stroke: currentColor; fill: none; stroke-width: 1.8; stroke-linecap: round; stroke-linejoin: round; }
            ul { list-style: none; padding: 0; }
            li { display: flex; align-items: center; gap: 12px; padding: 10px; border-bottom: 1px solid #eee; }
            li .name { flex: 1; }
            #form { display: flex; justify-content: center; gap: 8px; margin: 20px 0; }
            input { padding: 10px; border: 1px solid #bbb; border-radius: 8px; }
            [hidden] { display: none !important; }
          </style>
        </head>
        <body>
          <main>
            <section id="home">
              <h1>Svenska kräftor kräfva dessa drycker</h1>
              <div class="actions">
                <button class="icon-button" aria-label="Drycker" title="Drycker" onclick="showBeverages()"><svg viewBox="0 0 24 24"><path d="M7 3h10l-1 7a4 4 0 0 1-3 3.8V21h4v1H7v-1h4v-7.2A4 4 0 0 1 8 10L7 3Zm1.2 2 .6 5h6.4l.6-5H8.2Z"/><path d="M17 4h3v6a2 2 0 0 1-2 2h-1"/></svg></button>
              </div>
            </section>
            <section id="beverages" hidden>
              <h1>Drycker</h1>
              <div class="actions">
                <button class="icon-button" aria-label="Lägg till dryck" title="Lägg till dryck" onclick="showForm()"><svg viewBox="0 0 24 24"><path d="M12 5v14M5 12h14"/></svg></button>
              </div>
              <form id="form" hidden onsubmit="addBeverage(event)">
                <label for="beverage-name">Dryckens namn</label>
                <input id="beverage-name" required>
                <button type="submit">Spara</button>
              </form>
              <ul id="beverage-list"><li><span class="name">Testdryck</span><span>Öl</span></li></ul>
            </section>
          </main>
          <script>
            function showBeverages() { document.querySelector('#home').hidden = true; document.querySelector('#beverages').hidden = false; }
            function showForm() { document.querySelector('#form').hidden = false; document.querySelector('#beverage-name').focus(); }
            function addBeverage(event) {
              event.preventDefault();
              const name = document.querySelector('#beverage-name').value.trim();
              const row = document.createElement('li');
              const label = document.createElement('span');
              label.className = 'name'; label.textContent = name; row.append(label);
              document.querySelector('#beverage-list').append(row);
              document.querySelector('#form').reset();
              document.querySelector('#form').hidden = true;
            }
          </script>
        </body>
        </html>
        """;
}
