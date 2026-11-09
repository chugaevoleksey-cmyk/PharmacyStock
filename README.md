# PharmacyStock — лабораторна робота 4

Варіант 19. Чугаєв Олексій Михайлович, група 471.
Гілка: lab04-classes-solid.

## Запуск

Потрібні SDK із підтримкою net8.0, Runtime 8 та пакети з кешу попередніх робіт.

    dotnet restore
    dotnet build -c Release -warnaserror
    dotnet test -c Release
    dotnet test -c Release --filter FullyQualifiedName~DomainInvariantTests
    dotnet run --project PharmacyStock.App
    dotnet format --verify-no-changes

Загалом проходять 38 тестів: 35 із ЛР3 та рівно 3 нових із ЛР4.
Якщо кеш NuGet порожній:

    dotnet restore --source https://api.nuget.org/v3/index.json

## Демонстрація політик

    dotnet run --project PharmacyStock.App -- regular
    dotnet run --project PharmacyStock.App -- vip
    dotnet run --project PharmacyStock.App -- staff
    dotnet run --project PharmacyStock.App -- standard

Для двох одиниць по 250 очікувані суми: 475.00, 450.00, 350.00, 500.00.
Без аргументу обирається regular; для невідомого ключа — standard без знижки.
Загальне правило після підсумку: якщо сума > 1000, віднімається 50.
Ціни рядків округлюються до 2 знаків за правилом ToEven.

## Структура

- Core/Domain — Patient, Medicine, PrescriptionLine, Prescription та архівний знімок.
- Core/Abstractions — вузькі інтерфейси сховища, ціни та сповіщення.
- Core/Storage — словник у пам’яті без файлів.
- Core/Pricing — звичайна, знижкова та VIP-політика, підсумок вартості.
- Core/Services — сценарій збереження з залежностями в конструкторі.
- Core/Notifications — адаптер до переданого TextWriter, без справжніх листів.
- Core/Reporting/PrescriptionCsvReport.cs — повертає CSV, нічого не пише на диск.
- App/Program.cs — композиційний корінь і демонстрація.
- docs/model.md — модель, інваріанти, стани та навчальні припущення.
- docs/lab04 — аудит, журнали, результати та вихідний навчальний Legacy-приклад.

Prescription.Lines повертає read-only обгортку, а не прихований List через інтерфейс.
ArchivedPrescription фіксує стан і окремий список незмінних позицій у момент створення.
GetAll сховища повертає копію списку посилань; самі агрегати залишаються змінними.
Модулі Documents і розрахунок ЛР3 залишено для попередніх тестів; App тепер показує ЛР4.

Початковий клас-Бог був лише зразком для аудиту й не запускався.
Він збережений у Git та docs/lab04/legacy; у робочій бібліотеці Legacy немає.
Копія проєкту підготовлена окремо від каталогу на робочому столі.
