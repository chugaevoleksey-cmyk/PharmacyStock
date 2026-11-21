# PharmacyStock

![CI](https://github.com/chugaevoleksey-cmyk/PharmacyStock/actions/workflows/ci.yml/badge.svg?branch=main) — ЛР7, варіант 19
Чугаєв Олексій Михайлович, група 471. Гілка `lab07-tests`.

## Відкрити та перевірити
Розпакуйте у нову порожню папку й відкрийте у VS Code папку з PharmacyStock.sln.
```
dotnet build -c Release -warnaserror
dotnet test -c Release
dotnet test tests/PharmacyStock.Tests -c Release --filter "FullyQualifiedName~PrescriptionDiscountPolicyTests|FullyQualifiedName~PrescriptionServiceDoubleTests|FullyQualifiedName~UncoveredBranchTests"
```
Повне рішення: 80 тестів основного проєкту + 30 Legacy = 110, без пропущених. Лише нові тести ЛР7 — 19. Пакети xUnit 2.5.3, Test SDK 17.8.0 та Coverlet 6.0.0. Проєкти застосунку й тестів net8.0; встановлено SDK 10 і .NET/ASP.NET Core runtime 8.
Якщо на новому ПК немає кешу пакетів:
```
dotnet restore --source https://api.nuget.org/v3/index.json
```

## Що змінено
Нова `PrescriptionDiscountPolicy` розраховує промо-знижку на повну суму рецепта: постійний пацієнт 5%, сума від 1000 дає ще 10%, стеля 150 грн. Це окреме правило, а не непомітна заміна цін позицій або тарифів Legacy. Звичайний запуск PharmacyStock.App зберігає поведінку ЛР5; основний результат ЛР7 демонструється тестами.
Сервіс з ЛР5 уже отримує репозиторій, ціноутворення, повідомлення й ILogger через конструктор. Для дублерів його конструктор змінювати не знадобилося. `Pay` зберігає рецепт через Save, тому лічильник перевірено саме в Pay, а не в Place (Place виконує Add і Notify). Невідомий ID дає існуючий DomainRuleException, а не KeyNotFoundException зі зразка ShopOrders.

## Цикл TDD
Мітки `lab7-red`, `lab7-green`, `lab7-refactor` відповідають рівно трьом послідовним комітам. Червоний змінює лише файл тестів; зелений і рефакторинг — лише файл політики. У першій фазі 5000 × 15% = 750 замість потрібних 150, тест падає. Цей стан навмисний.
Відтворення (починайте з чистого git status):
```
git switch --detach lab7-red
dotnet test tests/PharmacyStock.Tests -c Release --filter FullyQualifiedName~DiscountFor_HugePrescription
git switch lab07-tests
```
Після демонстрації обов’язково поверніться до lab07-tests. Повні журнали та перевірка змінених файлів — docs/lab07/evidence.

## Покриття всього Core
```
dotnet test tests/PharmacyStock.Tests -c Release --collect:"XPlat Code Coverage" --settings coverage.runsettings
```
До додаткових тестів: 371/504 рядків = 73,61%, 75/90 гілок = 83,33%.
Після: 427/504 = 84,72%, 81/90 = 90%. Усього додано 6 тестів, основний — Pay_EmptyPrescription_RejectsWithoutSaving. Вимірюється вся збірка Core, старі класи не виключено.
Відкрийте coverage/index.html: власний HTML-переглядач зі справжнього XML Coverlet показує підсумок та виконані/невиконані рядки. ReportGenerator не встановлювався. Сирі XML з двох вимірювань збережено в docs/lab07/evidence, усі 7 частково непокритих точок (9 напрямків) пояснено в docs/lab07/coverage.md. Папки coverage/ і TestResults/ ігноруються Git.

## Репозиторій
https://github.com/chugaevoleksey-cmyk/PharmacyStock
Архів містить повну .git з історією ЛР1–ЛР7. На GitHub нова гілка в цій сесії не публікувалася. Для завантаження зі свого облікового запису:
```
git remote add origin https://github.com/chugaevoleksey-cmyk/PharmacyStock.git
git push -u origin lab07-tests
git push origin lab7 lab7-red lab7-green lab7-refactor lab7-coverage-before
```
Якщо origin вже додано, перевірте його через git remote -v та пропустіть remote add. Force push не потрібен.


## Лабораторна 8

Версія 0.8.0. SDK 8.0.427 задає global.json; NuGet.Config використовує nuget.org замість локального кешу.

```powershell
dotnet restore
dotnet format --verify-no-changes --no-restore
dotnet build -c Release --no-restore
dotnet test -c Release --no-restore /p:CollectCoverage=true /p:CoverletOutputFormat=cobertura /p:Threshold=60 /p:ThresholdType=line /p:ThresholdStat=total '/p:Include=[PharmacyStock.Core]*'
dotnet run --project PharmacyStock.App -c Release --no-build -- --demo
```

CI на push до main і feature/** та на PR до main перевіряє формат перед компіляцією, щоб швидко відхиляти порушення стилю. Складання трактує всі попередження як помилки; усі тести мають пройти, покриття рядків усієї PharmacyStock.Core — не менше 60%. Coverlet застосовано до основного тестового проєкту, 30 Legacy-тестів також запускаються; старі класи Core не виключено з покриття. XML зберігається артефактом coverage навіть у невдалому запуску. Аудит NuGet усіх залежностей активний при restore; діагностики NU1901–NU1904 зупиняють складання.

Спільний PrescriptionLineTextParser обмежує код 20 символами, кількість 1–1000, ціну невід'ємним decimal із крапкою без роздільників тисяч; перевіряє переповнення суми. Консольний адаптер використовує ті самі правила. Ключі API застосунку не потрібні; .env*, appsettings.Local.json та *.secrets.json ігноруються.
