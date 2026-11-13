# PharmacyStock — лабораторна робота №6, варіант 19

Чугаєв Олексій Михайлович, група 471. Гілка `lab6-refactoring`. Успадкований навчальний модуль ізольовано від сучасного Core у `src/Legacy`.

## Запуск
Розпакуйте архів у **нову порожню папку** і відкрийте корінь із PharmacyStock.sln у VS Code.
Встановлено .NET SDK 10 та runtime .NET/ASP.NET Core 8; застосунок і тести мають ціль net8.0.
```
dotnet build -c Release -warnaserror
dotnet test -c Release
dotnet run --project tools/LegacyDemo
```
Результат: 30 характеризаційних + 61 попередній тест = 91. Демонстрація дає VIP-звіт 655.00, 4 ERR, межу доставки 1000 та журнал. Фактичної розсилки пошти немає. Старий `PharmacyStock.App` демонструє ЛР5; для ЛР6 запускайте саме `tools/LegacyDemo`.

NuGet.Config виключає мережеві джерела і використовує вже заповнений кеш. Якщо пакети на новому комп’ютері відсутні:
```
dotnet restore --source https://api.nuget.org/v3/index.json
```

## Докази до і після
Стан до рефакторингів — мітка `lab6-before`; фінальна мітка — `lab6`. Початковий лістинг: docs/lab06/LegacyPrescriptionProcessor.before.cs.txt. Тест LegacyCharacterizationTests.cs не змінювався після початкового коміту; лише LegacyCalls адаптує нові типи та API.
```
git switch --detach lab6-before
dotnet test tests/Legacy.Tests -c Release
git switch lab6-refactoring
dotnet test tests/Legacy.Tests -c Release
```
В обох випадках мають пройти ті самі 30 випадків. Перед переходом стан Git має бути чистим. Десять refactor-комітів перевірено окремо; журнал і хеші — docs/lab06/history-verification.json. У всіх також проходять 61 попередній тест.

## Метрики
```
dotnet run --project tools/LegacyMetrics -- src/Legacy
```
Цей окремий службовий інструмент потребує SDK 10 / net10.0 (як у середовищі виконання), не включений до рішення й використовує Roslyn зі SDK без NuGet. Визначення LOC, CC і вкладеності, точки розгалуження та таблиці — docs/metrics.md, docs/lab06/metrics-before.json і metrics-after.json.
Handle: 80 → 16 LOC, CC 17 → 3, вкладеність 4 → 1, параметрів 11 → 1. Загальний LOC модуля: 149 → 156. Максимальна CC будь-якого методу після — 7 у Validate, а не 3: перевірки нікуди не зникли.

## Обмеження
Початковий код містить залежність від локалі та року, приймає від’ємні ціни, використовує рядкові ERR і фіксовані тарифи. Це поведінка, яку за умовою не змінюють. Реєстр із оцінкою 2850 грн — docs/tech-debt.md.
Аналізатори й стиль навмисно не застосовуються до ізольованого початкового legacy-проєкту; компілятор, nullable та TreatWarningsAsErrors лишаються. У сучасних Core/App суворі перевірки збережені. Не видаємо вимкнення аналізатора за відсутність code smells.

Репозиторій, наданий користувачем: https://github.com/chugaevoleksey-cmyk/PharmacyStock . Гілка ЛР6 підготовлена локально; завантаження гілки та міток на GitHub у цій сесії не виконувалося. Архів містить повну локальну історію .git.
Для публікації з кореня розпакованого проєкту (після входу у свій GitHub):
```
git remote add origin https://github.com/chugaevoleksey-cmyk/PharmacyStock.git
git push -u origin lab6-refactoring
git push origin lab6-before lab6
```
Ці команди додають нову гілку й мітки без force push. Якщо origin вже існує, спочатку перевірте `git remote -v`; повторно додавати його не потрібно.
