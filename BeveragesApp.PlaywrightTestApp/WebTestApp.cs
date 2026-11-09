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
                <button class="icon-button" aria-label="Kategorier" title="Kategorier" onclick="showCategories()"><svg viewBox="0 0 24 24"><path d="M4 5h7v6H4zM13 5h7v6h-7zM4 13h7v6H4zM13 13h7v6h-7z"/></svg></button>
                <button class="icon-button" aria-label="Länder" title="Länder" onclick="showCountries()"><svg viewBox="0 0 24 24"><circle cx="12" cy="12" r="9"/><path d="M3 12h18M12 3a15 15 0 0 1 0 18M12 3a15 15 0 0 0 0 18"/></svg></button>
                <button class="icon-button" aria-label="Graderingar" title="Graderingar" onclick="showRemarks()"><svg viewBox="0 0 24 24"><path d="m12 3 2.8 5.7 6.2.9-4.5 4.4 1.1 6.2-5.6-3-5.6 3 1.1-6.2L3 9.6l6.2-.9L12 3Z"/></svg></button>
                <button class="icon-button" aria-label="Avsluta" title="Avsluta" onclick="showExitConfirmation()"><svg viewBox="0 0 24 24"><path d="M10 4H4v16h6M14 8l4 4-4 4m4-4H9"/></svg></button>
              </div>
            </section>
            <section id="beverages" hidden>
              <div class="actions"><button class="icon-button" aria-label="Tillbaka" title="Tillbaka" onclick="showHome()"><svg viewBox="0 0 24 24"><path d="M19 12H5m0 0 7-7m-7 7 7 7"/></svg></button><button class="icon-button" aria-label="Avsluta" title="Avsluta" onclick="showExitConfirmation()"><svg viewBox="0 0 24 24"><path d="M10 4H4v16h6M14 8l4 4-4 4m4-4H9"/></svg></button></div>
              <h1>Drycker</h1>
              <div class="actions">
                <button class="icon-button" aria-label="Lägg till dryck" title="Lägg till dryck" onclick="openBeverage()"><svg viewBox="0 0 24 24"><path d="M12 5v14M5 12h14"/></svg></button>
              </div>
              <ul id="beverage-list"></ul>
            </section>
            <section id="beverage-item" hidden>
              <div class="actions"><button class="icon-button" aria-label="Tillbaka" title="Tillbaka" onclick="showBeverageList()"><svg viewBox="0 0 24 24"><path d="M19 12H5m0 0 7-7m-7 7 7 7"/></svg></button><button class="icon-button" aria-label="Avsluta" title="Avsluta" onclick="showExitConfirmation()"><svg viewBox="0 0 24 24"><path d="M10 4H4v16h6M14 8l4 4-4 4m4-4H9"/></svg></button></div>
              <h1>Dryck</h1>
              <label for="beverage-item-name">Dryckens namn</label>
              <input id="beverage-item-name" required>
              <div class="actions">
                <button class="icon-button" aria-label="Spara dryck" title="Spara" onclick="saveBeverage()"><svg viewBox="0 0 24 24"><path d="M5 3h12l4 4v14H3V3h2Zm2 0v6h10V3M7 21v-8h10v8"/></svg></button>
                <button class="icon-button" aria-label="Ta bort dryck" title="Ta bort" onclick="deleteBeverage()"><svg viewBox="0 0 24 24"><path d="M4 7h16M10 11v6m4-6v6M6 7l1 14h10l1-14M9 7V4h6v3"/></svg></button>
              </div>
            </section>
            <section id="categories" hidden>
              <div class="actions"><button class="icon-button" aria-label="Tillbaka" title="Tillbaka" onclick="showHome()"><svg viewBox="0 0 24 24"><path d="M19 12H5m0 0 7-7m-7 7 7 7"/></svg></button><button class="icon-button" aria-label="Avsluta" title="Avsluta" onclick="showExitConfirmation()"><svg viewBox="0 0 24 24"><path d="M10 4H4v16h6M14 8l4 4-4 4m4-4H9"/></svg></button></div>
              <h1>Dryckeskategorier</h1>
              <div class="actions">
                <button class="icon-button" aria-label="Lägg till kategori" title="Lägg till kategori" onclick="openCategory()"><svg viewBox="0 0 24 24"><path d="M12 5v14M5 12h14"/></svg></button>
              </div>
              <ul id="category-list"></ul>
            </section>
            <section id="category-item" hidden>
              <div class="actions"><button class="icon-button" aria-label="Tillbaka" title="Tillbaka" onclick="showCategoryList()"><svg viewBox="0 0 24 24"><path d="M19 12H5m0 0 7-7m-7 7 7 7"/></svg></button><button class="icon-button" aria-label="Avsluta" title="Avsluta" onclick="showExitConfirmation()"><svg viewBox="0 0 24 24"><path d="M10 4H4v16h6M14 8l4 4-4 4m4-4H9"/></svg></button></div>
              <h1>Dryckeskategori</h1>
              <form id="category-form" onsubmit="saveCategory(event)">
                <label for="category-id">Kategorins ID</label>
                <input id="category-id" disabled>
                <label for="category-name">Kategorins namn</label>
                <input id="category-name" required>
                <div class="actions">
                  <button class="icon-button" type="submit" aria-label="Spara kategori" title="Spara"><svg viewBox="0 0 24 24"><path d="M5 3h12l4 4v14H3V3h2Zm2 0v6h10V3M7 21v-8h10v8"/></svg></button>
                  <button class="icon-button" type="button" aria-label="Ta bort kategori" title="Ta bort" onclick="deleteCategory()"><svg viewBox="0 0 24 24"><path d="M4 7h16M10 11v6m4-6v6M6 7l1 14h10l1-14M9 7V4h6v3"/></svg></button>
                </div>
              </form>
            </section>
            <section id="countries" hidden>
              <div class="actions"><button class="icon-button" aria-label="Tillbaka" title="Tillbaka" onclick="showHome()"><svg viewBox="0 0 24 24"><path d="M19 12H5m0 0 7-7m-7 7 7 7"/></svg></button><button class="icon-button" aria-label="Avsluta" title="Avsluta" onclick="showExitConfirmation()"><svg viewBox="0 0 24 24"><path d="M10 4H4v16h6M14 8l4 4-4 4m4-4H9"/></svg></button></div>
              <h1>Länder</h1>
              <div class="actions">
                <button class="icon-button" aria-label="Lägg till land" title="Lägg till land" onclick="openCountry()"><svg viewBox="0 0 24 24"><path d="M12 5v14M5 12h14"/></svg></button>
              </div>
              <ul id="country-list"></ul>
            </section>
            <section id="country-item" hidden>
              <div class="actions"><button class="icon-button" aria-label="Tillbaka" title="Tillbaka" onclick="showCountryList()"><svg viewBox="0 0 24 24"><path d="M19 12H5m0 0 7-7m-7 7 7 7"/></svg></button><button class="icon-button" aria-label="Avsluta" title="Avsluta" onclick="showExitConfirmation()"><svg viewBox="0 0 24 24"><path d="M10 4H4v16h6M14 8l4 4-4 4m4-4H9"/></svg></button></div>
              <h1>Land</h1>
              <form id="country-form" onsubmit="saveCountry(event)">
                <label for="country-id">Landets ID</label>
                <input id="country-id" disabled>
                <label for="country-name">Landets namn</label>
                <input id="country-name" required>
                <div class="actions">
                  <button class="icon-button" type="submit" aria-label="Spara land" title="Spara"><svg viewBox="0 0 24 24"><path d="M5 3h12l4 4v14H3V3h2Zm2 0v6h10V3M7 21v-8h10v8"/></svg></button>
                  <button class="icon-button" type="button" aria-label="Ta bort land" title="Ta bort" onclick="deleteCountry()"><svg viewBox="0 0 24 24"><path d="M4 7h16M10 11v6m4-6v6M6 7l1 14h10l1-14M9 7V4h6v3"/></svg></button>
                </div>
              </form>
            </section>
            <section id="remarks" hidden>
              <div class="actions"><button class="icon-button" aria-label="Tillbaka" title="Tillbaka" onclick="showHome()"><svg viewBox="0 0 24 24"><path d="M19 12H5m0 0 7-7m-7 7 7 7"/></svg></button><button class="icon-button" aria-label="Avsluta" title="Avsluta" onclick="showExitConfirmation()"><svg viewBox="0 0 24 24"><path d="M10 4H4v16h6M14 8l4 4-4 4m4-4H9"/></svg></button></div>
              <h1>Graderingar</h1>
              <div class="actions">
                <button class="icon-button" aria-label="Lägg till gradering" title="Lägg till gradering" onclick="openRemark()"><svg viewBox="0 0 24 24"><path d="M12 5v14M5 12h14"/></svg></button>
              </div>
              <ul id="remark-list"></ul>
            </section>
            <section id="remark-item" hidden>
              <div class="actions"><button class="icon-button" aria-label="Tillbaka" title="Tillbaka" onclick="showRemarkList()"><svg viewBox="0 0 24 24"><path d="M19 12H5m0 0 7-7m-7 7 7 7"/></svg></button><button class="icon-button" aria-label="Avsluta" title="Avsluta" onclick="showExitConfirmation()"><svg viewBox="0 0 24 24"><path d="M10 4H4v16h6M14 8l4 4-4 4m4-4H9"/></svg></button></div>
              <h1>Gradering</h1>
              <form id="remark-form" onsubmit="saveRemark(event)">
                <label for="remark-id">Gradering ID</label>
                <input id="remark-id" disabled>
                <label for="remark-order">Graderingens ordningsnr</label>
                <input id="remark-order" type="number">
                <label for="remark-name">Graderingens namn</label>
                <input id="remark-name" required>
                <div class="actions">
                  <button class="icon-button" type="submit" aria-label="Spara gradering" title="Spara"><svg viewBox="0 0 24 24"><path d="M5 3h12l4 4v14H3V3h2Zm2 0v6h10V3M7 21v-8h10v8"/></svg></button>
                  <button class="icon-button" type="button" aria-label="Ta bort gradering" title="Ta bort" onclick="deleteRemark()"><svg viewBox="0 0 24 24"><path d="M4 7h16M10 11v6m4-6v6M6 7l1 14h10l1-14M9 7V4h6v3"/></svg></button>
                </div>
              </form>
            </section>
          </main>
          <dialog id="exit-dialog">
            <h2>Avsluta</h2>
            <p>Är du säker?</p>
            <button type="button" onclick="cancelExit()">Nej</button>
            <button type="button" onclick="confirmExit()">Ja</button>
          </dialog>
          <dialog id="error-dialog">
            <h2>Fel</h2>
            <p id="error-message"></p>
            <button type="button" onclick="closeError()">OK</button>
          </dialog>
          <script>
            function showExitConfirmation() { document.querySelector('#exit-dialog').showModal(); }
            function cancelExit() { document.querySelector('#exit-dialog').close(); }
            function confirmExit() { document.querySelector('#exit-dialog').close(); document.querySelector('#home h1').textContent = 'Avslut bekräftat'; }
            function showUnsavedDeleteError(message) { document.querySelector('#error-message').textContent = message; document.querySelector('#error-dialog').showModal(); }
            function closeError() { document.querySelector('#error-dialog').close(); }
            let beverages = [{ name: 'Testdryck', category: 'Öl' }];
            let selectedBeverage = null;
            function showBeverages() { document.querySelectorAll('main > section').forEach(section => section.hidden = true); document.querySelector('#beverages').hidden = false; renderBeverages(); }
            function showBeverageList() { document.querySelector('#beverage-item').hidden = true; document.querySelector('#beverages').hidden = false; renderBeverages(); }
            function renderBeverages() {
              const list = document.querySelector('#beverage-list'); list.replaceChildren();
              beverages.forEach((beverage, index) => {
                const row = document.createElement('li');
                const label = document.createElement('span'); label.className = 'name'; label.textContent = beverage.name; row.append(label);
                const category = document.createElement('span'); category.textContent = beverage.category; row.append(category);
                const edit = document.createElement('button'); edit.className = 'icon-button'; edit.setAttribute('aria-label', `Ändra dryck ${beverage.name}`); edit.title = 'Ändra'; edit.innerHTML = '<svg viewBox="0 0 24 24"><path d="m14 5 5 5M4 20l4.5-1 11-11a2.1 2.1 0 0 0-3-3l-11 11L4 20Z"/></svg>'; edit.onclick = () => openBeverage(index); row.append(edit);
                list.append(row);
              });
            }
            function openBeverage(index = null) {
              selectedBeverage = index;
              document.querySelector('#beverage-item-name').value = index === null ? '' : beverages[index].name;
              document.querySelector('#beverages').hidden = true;
              document.querySelector('#beverage-item').hidden = false;
            }
            function saveBeverage() {
              const name = document.querySelector('#beverage-item-name').value.trim();
              if (selectedBeverage === null) beverages.push({ name, category: 'Öl' });
              else beverages[selectedBeverage].name = name;
              showBeverageList();
            }
            function deleteBeverage() {
              if (selectedBeverage === null) { showUnsavedDeleteError('Drycken har inte sparats och kan därför inte tas bort.'); return; }
              beverages.splice(selectedBeverage, 1);
              showBeverageList();
            }
            let categories = ['Öl', 'Vin'];
            let selectedCategory = null;
            function showHome() { document.querySelectorAll('main > section').forEach(section => section.hidden = true); document.querySelector('#home').hidden = false; }
            function showCategories() { document.querySelectorAll('main > section').forEach(section => section.hidden = true); document.querySelector('#categories').hidden = false; renderCategories(); }
            function showCategoryList() { document.querySelector('#category-item').hidden = true; document.querySelector('#categories').hidden = false; renderCategories(); }
            function renderCategories() {
              const list = document.querySelector('#category-list'); list.replaceChildren();
              categories.forEach((name, index) => {
                const row = document.createElement('li');
                const label = document.createElement('span'); label.className = 'name'; label.textContent = name; row.append(label);
                const edit = document.createElement('button'); edit.className = 'icon-button'; edit.setAttribute('aria-label', `Ändra kategori ${name}`); edit.title = 'Ändra'; edit.innerHTML = '<svg viewBox="0 0 24 24"><path d="m14 5 5 5M4 20l4.5-1 11-11a2.1 2.1 0 0 0-3-3l-11 11L4 20Z"/></svg>'; edit.onclick = () => openCategory(index); row.append(edit);
                list.append(row);
              });
            }
            function openCategory(index = null) {
              selectedCategory = index;
              document.querySelector('#category-id').value = index === null ? '' : String(index + 1);
              document.querySelector('#category-name').value = index === null ? '' : categories[index];
              document.querySelector('#categories').hidden = true;
              document.querySelector('#category-item').hidden = false;
            }
            function saveCategory(event) {
              event.preventDefault();
              const name = document.querySelector('#category-name').value.trim();
              if (selectedCategory === null) categories.push(name); else categories[selectedCategory] = name;
              showCategoryList();
            }
            function deleteCategory() {
              if (selectedCategory === null) { showUnsavedDeleteError('Kategorin har inte sparats och kan därför inte tas bort.'); return; }
              categories.splice(selectedCategory, 1);
              showCategoryList();
            }
            let countries = ['Sverige', 'Norge', 'Danmark', 'Finland', 'Italien', 'Tyskland', 'Spanien', 'Sydafrika'];
            let selectedCountry = null;
            function showCountries() { document.querySelectorAll('main > section').forEach(section => section.hidden = true); document.querySelector('#countries').hidden = false; renderCountries(); }
            function showCountryList() { document.querySelector('#country-item').hidden = true; document.querySelector('#countries').hidden = false; renderCountries(); }
            function renderCountries() {
              const list = document.querySelector('#country-list'); list.replaceChildren();
              countries.forEach((name, index) => {
                const row = document.createElement('li');
                const label = document.createElement('span'); label.className = 'name'; label.textContent = name; row.append(label);
                const edit = document.createElement('button'); edit.className = 'icon-button'; edit.setAttribute('aria-label', `Ändra land ${name}`); edit.title = 'Ändra'; edit.innerHTML = '<svg viewBox="0 0 24 24"><path d="m14 5 5 5M4 20l4.5-1 11-11a2.1 2.1 0 0 0-3-3l-11 11L4 20Z"/></svg>'; edit.onclick = () => openCountry(index); row.append(edit);
                list.append(row);
              });
            }
            function openCountry(index = null) {
              selectedCountry = index;
              document.querySelector('#country-id').value = index === null ? '' : String(index + 1);
              document.querySelector('#country-name').value = index === null ? '' : countries[index];
              document.querySelector('#countries').hidden = true;
              document.querySelector('#country-item').hidden = false;
            }
            function saveCountry(event) {
              event.preventDefault();
              const name = document.querySelector('#country-name').value.trim();
              if (selectedCountry === null) countries.push(name); else countries[selectedCountry] = name;
              showCountryList();
            }
            function deleteCountry() {
              if (selectedCountry === null) { showUnsavedDeleteError('Landet har inte sparats och kan därför inte tas bort.'); return; }
              countries.splice(selectedCountry, 1);
              showCountryList();
            }
            let remarks = [
              { name: 'Hemskt', order: 1 },
              { name: 'Inte bra', order: 2 },
              { name: 'Medelmåttligt', order: 3 },
              { name: 'Bra', order: 4 },
              { name: 'Strålande', order: 5 }
            ];
            let selectedRemark = null;
            function showRemarks() { document.querySelectorAll('main > section').forEach(section => section.hidden = true); document.querySelector('#remarks').hidden = false; renderRemarks(); }
            function showRemarkList() { document.querySelector('#remark-item').hidden = true; document.querySelector('#remarks').hidden = false; renderRemarks(); }
            function renderRemarks() {
              const list = document.querySelector('#remark-list'); list.replaceChildren();
              remarks.forEach((remark, index) => {
                const row = document.createElement('li');
                const label = document.createElement('span'); label.className = 'name'; label.textContent = remark.name; row.append(label);
                const edit = document.createElement('button'); edit.className = 'icon-button'; edit.setAttribute('aria-label', `Ändra gradering ${remark.name}`); edit.title = 'Ändra'; edit.innerHTML = '<svg viewBox="0 0 24 24"><path d="m14 5 5 5M4 20l4.5-1 11-11a2.1 2.1 0 0 0-3-3l-11 11L4 20Z"/></svg>'; edit.onclick = () => openRemark(index); row.append(edit);
                list.append(row);
              });
            }
            function openRemark(index = null) {
              selectedRemark = index;
              document.querySelector('#remark-id').value = index === null ? '' : String(index + 1);
              document.querySelector('#remark-order').value = index === null ? '' : String(remarks[index].order);
              document.querySelector('#remark-name').value = index === null ? '' : remarks[index].name;
              document.querySelector('#remarks').hidden = true;
              document.querySelector('#remark-item').hidden = false;
            }
            function saveRemark(event) {
              event.preventDefault();
              const name = document.querySelector('#remark-name').value.trim();
              const orderText = document.querySelector('#remark-order').value;
              if (selectedRemark === null) remarks.push({ name, order: Number(orderText) || remarks.length + 1 });
              else remarks[selectedRemark] = { name, order: Number(orderText) || remarks[selectedRemark].order };
              showRemarkList();
            }
            function deleteRemark() {
              if (selectedRemark === null) { showUnsavedDeleteError('Graderingen har inte sparats och kan därför inte tas bort.'); return; }
              remarks.splice(selectedRemark, 1);
              showRemarkList();
            }
          </script>
        </body>
        </html>
        """;
}
