# SOLID — було та стало

| Принцип | Було | Стало |
|---|---|---|
| SRP | PrescriptionManager обчислює, зберігає, сповіщає та пише CSV | PrescriptionService, InMemoryPrescriptionRepository, TextWriterNotifier, PrescriptionCsvReport |
| OCP | if за patientType | IPricingPolicy; RegularPricingPolicy, DiscountPricingPolicy, VipPricingPolicy; вибір лише в Program |
| LSP | ArchivedPrescription успадковує AddLine і кидає NotSupportedException | Композиція з Prescription; зафіксований стан і копія позицій; AddLine відсутній |
| ISP | IPrescriptionStore має 7 методів; потрібен 1 | IPrescriptionRepository має 3; INotifier має 1; IPricingPolicy має 1 |
| DIP | Конкретні залежності створюються в полях менеджера | Інтерфейси передаються в конструктор; поля readonly; конкретні адаптери створено в Program |

Додавання політики: новий клас і один запис у таблиці Program; сервіс не змінюється.
Архів не підміняє рецепт у коді, який очікує змінюваний рецепт.
Збереження в пам’яті та запис сповіщення не є єдиною транзакцією; помилку адаптера
після збереження не слід трактувати як відкат сховища.
