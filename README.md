# PharmacyStock

Навчальний проєкт з конструювання програмного забезпечення. Варіант 19.
Автор звіту: Чугаєв Олексій Михайлович, група 471.

## Склад
- PharmacyStock.Core/Domain — модель із ЛР1.
- PharmacyStock.Core/Legacy/Prescription.cs — приведений до стандарту успадкований код ЛР2.
- PharmacyStock.App — приклад запуску ЛР2.
- docs/lab02 — початковий код, таблиці, журнали перевірок і вивід до та після.

## Вимоги й запуск
.NET SDK із підтримкою net8.0, .NET Runtime 8 та Git.
StyleCop.Analyzers 1.2.0-beta.556 потрібно мати в кеші NuGet.
NuGet.Config використовує локальний кеш без зовнішніх джерел.
Якщо пакета немає, додайте офіційне джерело NuGet у своїй копії конфігурації.

    dotnet restore
    dotnet build -warnaserror
    dotnet run --project PharmacyStock.App
    dotnet format --verify-no-changes

Програма фіксує uk-UA для відтворюваного виводу. Реєстр боргу: docs/lab02/technical-debt.md.
Фінансова логіка успадкована з методички та не є реалізацією правил реальної аптеки.
Правила стилю: CODING_STANDARD.md і .editorconfig.

Усі зміни ЛР2 підготовлено в окремій копії; вихідний каталог на робочому столі не змінювався.

Додаткова перевірка поведінки:

    dotnet run --project docs/lab02/verification-tools/BehaviorCheck.csproj

Перевірка містить вихідний навмисно неохайний код, тому його попередження не належать фінальній збірці рішення.
