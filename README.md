# PharmacyStock — лабораторна робота 5

Варіант 19. Чугаєв Олексій Михайлович, група 471.
Гілка: lab05-error-handling.

## Вимоги

SDK із підтримкою net8.0, .NET Runtime 8 і ASP.NET Core Runtime 8.
ILogger взято зі встановленого Microsoft.AspNetCore.App через FrameworkReference.
Ті самі стандартні ILogger і Console-провайдер працюють без завантаження нових пакетів NuGet.
Пакети тестів xUnit та StyleCop залишено з попередніх робіт.

## Перевірка

    dotnet restore
    dotnet build -c Release -warnaserror
    dotnet test -c Release
    dotnet test -c Release --filter FullyQualifiedName~ErrorHandlingTests
    dotnet format --verify-no-changes

Якщо кеш NuGet порожній:

    dotnet restore --source https://api.nuget.org/v3/index.json

## Демонстрація без ручного введення

    dotnet run --project PharmacyStock.App -- --demo
    dotnet run --project PharmacyStock.App -- --demo-error

Перша команда передає «дві», показує помилку, повторює ввід і додає 2 одиниці по 250.
Сума — 475.00; демонстраційна оплата 485 створює Warning про переплату 10.
Друга додатково пробує експорт у відсутній каталог, показує Error зі стеком і обробляє
контрольований IOException. Це навмисний збій, після якого програма продовжує роботу.
Після закриття аудиту програма перевіряє повторне ексклюзивне відкриття файла.

## Звичайний ввід

    dotnet run --project PharmacyStock.App

Введіть код, кількість і ціну окремими рядками. Десятковий роздільник — крапка.
Після невдалого вводу вводяться всі три поля ще раз. EOF завершує ввід без аварії.
У прикладі платіж автоматичний із переплатою 10; це демонстрація рівнів журналу,
а не інтеграція із платіжною системою.

## Результати та файли

runtime/audit.log містить UTC-час, операцію й номер рецепта.
runtime/report.txt містить експорт. runtime/ і файли журналів ігноруються Git
і не входять у ZIP. Збережені текстові виводи команд у docs/lab05 — докази перевірки.

- App/Input — TryParse і Result, сирі рядки залишаються поза Core.
- Core/Errors — Result<T> та DomainRuleException з Rule, PrescriptionId і InnerException.
- Core/Services — типізоване додавання, Load, Pay, Cancel; IOException не проковтується.
- Core/Logging — LoggerMessage.Define, іменовані поля та область PrescriptionId.
- App/Infrastructure — FileAuditLog і ReportExporter з гарантованим Dispose.
- App/Program.cs — єдиний широкий catch (Exception), рівні завершення 0/1/2/3.
- docs/errors.md — 9 ситуацій, по 3 на клас.
- docs/lab05 — таблиці, початковий анти-приклад, журнали й звіт перевірок.

GetById сховища залишається nullable як запит пошуку. Сервіс Load натомість або
повертає рецепт, або кидає DomainRuleException з контекстом. Result.Value на помилці
не повертає null, а відхиляє неправильне використання.

Сховище — у пам’яті. У тесті його замінено адаптером, який кидає IOException,
щоб перевірити журнал і збереження стека. Не стверджується, що реалізовано JSON-сховище
чи автоматичні повтори запису. Зміна агрегату й Save не є транзакцією з відкатом.
Листи не надсилаються. Початкові антипатерни збережено у Git і текстовому знімку,
але в робочому коді їх немає. Тести ЛР3 і ЛР4 збережено.
