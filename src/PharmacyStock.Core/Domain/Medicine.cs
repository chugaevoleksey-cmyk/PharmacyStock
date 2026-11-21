namespace PharmacyStock.Core.Domain;
/// <summary>Умовний препарат каталогу; ціна фіксується при створенні позиції рецепта.</summary>
/// <param name="Code">Унікальний непорожній код.</param>
/// <param name="Name">Умовна назва без персональних даних.</param>
/// <param name="Price">Додатна ціна одиниці у гривнях.</param>
/// <param name="Rules">Навчальні обмеження відпуску.</param>
public sealed record Medicine(string Code, string Name, decimal Price, MedicineRules Rules);

/// <summary>Навчальний ліміт одиниць на відпуск; не медична рекомендація.</summary>
/// <param name="RequiresPrescription">Чи обов'язковий підтверджений рецепт.</param>
/// <param name="MaximumUnits">Додатна верхня межа одиниць.</param>
public sealed record MedicineRules(bool RequiresPrescription, int MaximumUnits);
