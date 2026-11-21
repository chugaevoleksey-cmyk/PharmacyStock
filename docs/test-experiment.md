# Перевірка чутливості тестів
Константу DispensingPolicy.PrescriptionDays тимчасово змінено з 30 на 29.
Три випадки Dispense_ValidBoundary_ChargesAndCloses з віком 30 днів стали
червоними (кількість 1, 2 та 10). Решта випадків залишилися зеленими.
Константу повернуто до 30, повторний прогін повністю зелений.
Докази: docs/sr03/evidence/mutation-red.txt і mutation-restored.txt.
Змінений варіант правила не включено до головної гілки.
