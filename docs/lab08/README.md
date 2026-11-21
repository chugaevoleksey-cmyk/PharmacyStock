# Лабораторна 8

Варіант 19 PharmacyStock. Усі 132 тести проходять; поріг покриття рядків усієї Core становить 60%.

- [Початковий успішний CI №6](https://github.com/chugaevoleksey-cmyk/PharmacyStock/actions/runs/44919064534)
- [Навмисний збій CS0168, CI №10](https://github.com/chugaevoleksey-cmyk/PharmacyStock/actions/runs/44919575957)
- [PR із самооглядом](https://github.com/chugaevoleksey-cmyk/PharmacyStock/pull/1)

Початковий аудит NU1903 виявив транзитивні System.Net.Http 4.3.0 і System.Text.RegularExpressions 4.3.0. Оновлення xUnit 2.5.3 до 2.9.3 прибрало застарілий ланцюжок. Успішний аудит CI не знайшов відомих вразливостей; команда outdated окремо показує доступні оновлення, що не рівнозначно вразливості.

Тимчасовий Describe зі string note видалено окремим fix(ci). Перша спроба демонстрації також порушила SA1201; після впорядкування членів отримано ізольовану CS0168 на Build Release. Правила не послаблялися.

Форматувальник .NET 8 іноді втрачає пошук CLI (dotnet/sdk#44957): дозволено один повтор тільки коду 4. Порушення стилю повертають свій код одразу. Токен CI має лише contents: read.
