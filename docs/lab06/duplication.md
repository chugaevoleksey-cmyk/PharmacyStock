# Нормалізоване дублювання
Порівняно фрагменти `_tmpDiscount = 0m` до доставки в Handle та `decimal d = 0m` до доставки в Preview. Назви локальних змінних уніфіковано; початковий decimal біля d прибрано для порівняння з полем. Порожні рядки не враховано.
Однакових рядків: 16; із них без рядків лише з дужками: 10. Це нормалізоване, а не побайтове дублювання. Фрагмент тепер існує в одному LegacyDiscountPolicy.For, дубльованих копій алгоритму немає.
```
discount = 0m;
if (kind == "vip")
{
discount = subtotal * 0.15m;
if (discount > 500m) discount = 500m;
}
else if (kind == "staff")
{
discount = subtotal * 0.30m;
if (discount > 500m) discount = 500m;
}
else if (done > 10)
{
discount = subtotal * 0.05m;
if (discount > 500m) discount = 500m;
}
```
