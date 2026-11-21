# Машинна частина стандарту

Останнє значення ключа в області дії переважає попереднє. Наприклад, csharp_prefer_braces=true:error у фінальній секції перекриває початкове true:warning. Naming symbols і style самі по собі не є діагностикою; без зв’язаної naming rule вони нічого не контролюють.

| Секція | Ключ | Значення | Рівень | Зв’язок |
|---|---|---|---|---|
| root | root | true | параметр | F-03 — зупиняє пошук батьківських editorconfig |
| [*] | charset | utf-8 | параметр | F-03 — кодування UTF-8 |
| [*] | insert_final_newline | true | параметр | F-03 — новий рядок у кінці |
| [*] | trim_trailing_whitespace | true | параметр | F-03 — видалення хвостових пробілів |
| [*] | indent_style | space | параметр | F-01 — відступ пробілами |
| [*] | indent_size | 4 | параметр | F-01 — кількість пробілів у поточній секції |
| [*.{csproj,props,targets,json,yml}] | indent_size | 2 | параметр | F-01 — кількість пробілів у поточній секції |
| [*.cs] | max_line_length | 100 | параметр | F-04 — підказка довжини; не error компілятора |
| [*.cs] | csharp_new_line_before_open_brace | all | параметр | F-02 — дужки Allman |
| [*.cs] | csharp_new_line_before_else | true | параметр | F-02 — else на новому рядку |
| [*.cs] | csharp_new_line_before_catch | true | параметр | F-02 — catch на новому рядку |
| [*.cs] | csharp_prefer_braces | true:warning | warning | S-02 — перевага дужок; останнє значення error |
| [*.cs] | csharp_indent_case_contents | true | параметр | F-01 — відступ тіла case |
| [*.cs] | dotnet_sort_system_directives_first | true | параметр | F-09 — System перед іншими using |
| [*.cs] | csharp_style_namespace_declarations | file_scoped:warning | warning | F-08 — file-scoped namespace |
| [*.cs] | csharp_style_var_for_built_in_types | false:suggestion | suggestion | D-03 — явний вбудований тип, suggestion |
| [*.cs] | dotnet_naming_rule.priv_underscore.symbols | priv_field | параметр | N-08 — посилання на групу символів |
| [*.cs] | dotnet_naming_rule.priv_underscore.style | underscore_camel | параметр | N-08 — посилання на стиль |
| [*.cs] | dotnet_naming_rule.priv_underscore.severity | warning | warning | N-08 — рівень правила іменування |
| [*.cs] | dotnet_naming_symbols.priv_field.applicable_kinds | field | параметр | N-08 — види символів, до яких застосовується правило |
| [*.cs] | dotnet_naming_symbols.priv_field.applicable_accessibilities | private | параметр | N-08 — область видимості символів |
| [*.cs] | dotnet_naming_style.underscore_camel.capitalization | camel_case | параметр | N-08 — регістр назви |
| [*.cs] | dotnet_naming_style.underscore_camel.required_prefix | _ | параметр | N-08 — обов’язковий префікс назви |
| [*.cs] | dotnet_diagnostic.SA1633.severity | none | none | C-06 — заголовок ліцензії не потрібний для навчального проєкту |
| [*.cs] | dotnet_diagnostic.SA1101.severity | none | none | N-08 — this. не вимагається |
| [*.cs] | dotnet_diagnostic.SA1200.severity | none | none | F-09 — using лишаються зовні namespace |
| [*.cs] | dotnet_diagnostic.SA1309.severity | none | none | N-08 — префікс _ узгоджено з власною naming rule; SA1309 його не вимагає |
| [*.cs] | dotnet_diagnostic.SA0001.severity | none | none | C-07 — XML ввімкнено властивістю проєкту; окремий дубль перевірки не потрібен |
| [*.cs] | dotnet_diagnostic.SA1600.severity | warning | warning | C-07 — документація публічних типів |
| [*.cs] | dotnet_diagnostic.SA1601.severity | warning | warning | C-07 — документація partial типів |
| [*.cs] | dotnet_diagnostic.SA1602.severity | warning | warning | C-07 — документація enum |
| [*.cs] | dotnet_diagnostic.SA1413.severity | none | none | F-10 — кінцева кома необов’язкова |
| [*.cs] | dotnet_diagnostic.SA1028.severity | warning | warning | F-03 — хвостові пробіли |
| [*.cs] | dotnet_diagnostic.SA1201.severity | warning | warning | F-07 — порядок членів |
| [*.cs] | dotnet_diagnostic.SA1503.severity | warning | warning | S-02 — дужки контрольованого блока |
| [*.cs] | dotnet_diagnostic.SA1516.severity | warning | warning | F-05 — розділення членів порожнім рядком |
| [*.cs] | dotnet_diagnostic.IDE0055.severity | warning | warning | F-01 — форматування |
| [*.cs] | dotnet_naming_rule.constants_pascal.symbols | constant_fields | параметр | N-09 — посилання на групу символів |
| [*.cs] | dotnet_naming_rule.constants_pascal.style | pascal_case | параметр | N-09 — посилання на стиль |
| [*.cs] | dotnet_naming_rule.constants_pascal.severity | warning | warning | N-09 — рівень правила іменування |
| [*.cs] | dotnet_naming_symbols.constant_fields.applicable_kinds | field | параметр | N-09 — види символів, до яких застосовується правило |
| [*.cs] | dotnet_naming_symbols.constant_fields.required_modifiers | const | параметр | N-09 — відбір констант за модифікатором const |
| [*.cs] | dotnet_naming_style.pascal_case.capitalization | pascal_case | параметр | N-09 — регістр назви |
| [PharmacyStock.Core/Errors/Result.cs] | dotnet_diagnostic.CA1000.severity | none | none | C-06 — лише Result.cs: статичні фабрики generic Result<T> є контрактом |
| [tests/PharmacyStock.Tests/*Tests.cs] | dotnet_diagnostic.CA1707.severity | none | none | N-13 — лише основні *Tests.cs: підкреслення в назвах сценаріїв |
| [*.cs] | dotnet_diagnostic.CA1305.severity | warning | warning | D-13 — явна культура форматування |
| [*.cs] | end_of_line | lf | параметр | F-03 — LF |
| [*.cs] | csharp_prefer_braces | true:error | error | S-02 — перевага дужок; останнє значення error |
| [*.cs] | dotnet_diagnostic.IDE0011.severity | error | error | S-02 — дужки обов’язкові |
| [PharmacyStock.Core/**/*.cs] | dotnet_diagnostic.CS1591.severity | error | error | C-07 — лише Core: відсутній XML public API |

## Властивості складання

Nullable=enable — перевіряє nullability. TreatWarningsAsErrors=true — перетворює попередження на помилки. EnforceCodeStyleInBuild=true — залучає підтримані IDE-правила в build. AnalysisMode=Recommended та AnalysisLevel=8.0-recommended — фіксують рекомендований набір .NET 8. GenerateDocumentationFile=true лише для Core — створює XML і робить доступною CS1591, не вимагаючи документації кожної тестової допоміжної сутності. LangVersion=12.0 фіксує синтаксис; TargetFramework=net8.0 — API платформи; EnableNETAnalyzers=true — аналізатори .NET; ImplicitUsings=enable — стандартні using. VersionPrefix, Authors, Product задають метадані. NuGetAudit=true та NuGetAuditMode=all зберігають аудит усіх залежностей ЛР8.

StyleCop.Analyzers 1.2.0-beta.556 уже присутній у Core й App з PrivateAssets=all. Його не встановлювали вдруге. Допоміжний tools/MetricsTool має окремі послаблення, не входить у production scope і не доводить якість бібліотеки.

Перед комітом: `dotnet format --verify-no-changes --no-restore`, далі `dotnet build -c Release --no-restore -warnaserror` після restore. Позитивний код 0 потрібний від обох команд. У звичайному середовищі спочатку `dotnet restore`; локальні журнали цього завдання використовують вже перевірений кеш пакетів, а віддалений CI відновлює з nuget.org.

Доказ: тимчасове StandardProbe.Total як public mutable field викликало CA1051 і SA1401. Probe видалено, а не залишено у solution із придушенням. Після виправлення Release має 0 warnings / 0 errors, формат чистий, 102+30 тестів успішні. Файли журналів у evidence відображають справжні команди.
