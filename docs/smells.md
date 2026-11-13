# Реєстр дефектів до рефакторингу

Місця відповідають початковому src/Legacy/LegacyPrescriptionProcessor.cs; незмінна копія — docs/lab06/LegacyPrescriptionProcessor.before.cs.txt. Порядок — пріоритет подальшого розділення коду.

| № | Дефект | Місце | Числовий прояв | Рефакторинг |
|---|---|---|---|---|
| 1 | Дублювання | Handle 68; Preview 120 | Два блоки повторюють 3 знижки, 3 обмеження та доставку | Extract Method; спільний розрахунок |
| 2 | Довгий метод | Handle 30 | 11 параметрів; перевірка, сума, знижка, доставка, звіт і журнал в одному методі | Extract Method |
| 3 | Великий клас | LegacyPrescriptionProcessor 24 | 7 відповідальностей: валідація, сума, знижки, доставка, звіт, журнал, опис пацієнта | Extract Class |
| 4 | Довгий список параметрів | Handle 30 | 11 параметрів; 6 описують клієнта, createdAt не читається | Introduce Parameter Object |
| 5 | Одержимість елементарними типами | Handle 40; Preview 75 | Стан і вид клієнта — string; 2 дозволені стани і 3 гілки знижки | Replace Primitive with Enum |
| 6 | Заздрість до чужих даних | DescribeClient 146 | Читає 5 полів LegacyPatient і 0 полів процесора | Move Method |
| 7 | Вкладені умови | Handle 36 | 4 вкладені if до журналювання ok | Decompose Conditional; guard clauses |
| 8 | Тимчасове поле | поля 26, 27 | 2 поля використовуються лише під час Handle | Remove Temporary Field |
| 9 | Магічні числа | Handle 77; Preview | 7 різних бізнес-значень: 0.15, 0.30, 0.05, 500, 1000, 60, 10 | Replace Magic Number with Constant |
