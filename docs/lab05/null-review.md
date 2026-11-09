# Перегляд nullable-повернень

| Метод | Рішення | Пояснення |
|---|---|---|
| InMemoryPrescriptionRepository.GetById | Залишено Prescription? | Відсутність запису є нормальним результатом низькорівневого пошуку; контракт явно nullable. |
| PrescriptionService.Load | Замінено nullable/прихований збій на Prescription або DomainRuleException | Прикладний сценарій вимагає наявного рецепта й отримує Rule та PrescriptionId. |
| Result<T>.Value | Успіх повертає T; помилка доступу кидає InvalidOperationException | Помилка не маскується під null; спочатку перевіряється IsSuccess. |

У поточній бібліотеці інших методів пошуку з nullable-поверненням немає.
Nullable CouponCode є необов’язковим входом, а не помилкою пошуку.
