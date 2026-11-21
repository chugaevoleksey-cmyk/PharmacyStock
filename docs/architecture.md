# Архітектура
Архітектура продовжує лабораторні: агрегат Prescription, PrescriptionLine,
PrescriptionStatus та DomainRuleException перенесено зі збереженням історії.
Попередні навчальні калькулятори й тести збережені в archive/labs як .txt,
вони не компілюються і не входять до метрик випуску 1.0.

|Шар|Проєкт|Відповідальність|Заборонено|
|---|---|---|---|
|Домен|Core/Domain|Документи, партії, стани|Файли, консоль, JSON|
|Прикладний|Core/Application|Шість сценаріїв, валідація|Формат файла та Console|
|Інфраструктура|Infrastructure|JSON, системний час, файл журналу|Правила відпуску|
|Консоль|App|Ввід, вивід, створення залежностей|Формули й списання запасу|

App → Infrastructure → Core; App → Core.
Core не має ProjectReference та залежності від Microsoft.Extensions.Logging.
Компілятор не дає Core викликати класи Infrastructure; архітектурний тест
додатково перевіряє System.Console, System.IO і Json у Domain/Application.

Точки розширення: IDispensingPolicy — нове правило відпуску;
IStockRepository — інший формат зберігання; IClock — інший календар зміни.
Композиційний корінь Program.Main створює конкретні реалізації один раз.
