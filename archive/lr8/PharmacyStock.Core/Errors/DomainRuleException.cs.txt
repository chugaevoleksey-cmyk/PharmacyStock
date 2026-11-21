namespace PharmacyStock.Core.Errors;

/// <summary>Represents the DomainRuleException type.</summary>
public sealed class DomainRuleException : Exception
{
    /// <summary>Initializes a new instance of the <see cref="DomainRuleException"/> class.</summary>
    public DomainRuleException()
        : this("domain.unspecified", "Порушено правило предметної області.")
    {
    }

    /// <summary>Initializes a new instance of the <see cref="DomainRuleException"/> class.</summary>
    /// <param name="message">The message argument.</param>
    public DomainRuleException(string message)
        : this("domain.unspecified", message)
    {
    }

    /// <summary>Initializes a new instance of the <see cref="DomainRuleException"/> class.</summary>
    /// <param name="message">The message argument.</param>
    /// <param name="innerException">The innerException argument.</param>
    public DomainRuleException(string message, Exception innerException)
        : this("domain.unspecified", message, null, innerException)
    {
    }

    /// <summary>Initializes a new instance of the <see cref="DomainRuleException"/> class.</summary>
    /// <param name="rule">The rule argument.</param>
    /// <param name="message">The message argument.</param>
    public DomainRuleException(string rule, string message)
        : this(rule, message, null, null)
    {
    }

    /// <summary>Initializes a new instance of the <see cref="DomainRuleException"/> class.</summary>
    /// <param name="rule">The rule argument.</param>
    /// <param name="message">The message argument.</param>
    /// <param name="innerException">The innerException argument.</param>
    public DomainRuleException(string rule, string message, Exception innerException)
        : this(rule, message, null, innerException)
    {
    }

    /// <summary>Initializes a new instance of the <see cref="DomainRuleException"/> class.</summary>
    /// <param name="rule">The rule argument.</param>
    /// <param name="message">The message argument.</param>
    /// <param name="prescriptionId">The prescriptionId argument.</param>
    public DomainRuleException(string rule, string message, int prescriptionId)
        : this(rule, message, prescriptionId, null)
    {
    }

    private DomainRuleException(
        string rule, string message, int? prescriptionId, Exception? innerException)
        : base(message, innerException)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(rule);
        ArgumentException.ThrowIfNullOrWhiteSpace(message);
        Rule = rule;
        PrescriptionId = prescriptionId;
    }

    /// <summary>Gets the Rule value.</summary>
    public string Rule { get; }

    /// <summary>Gets the PrescriptionId value.</summary>
    public int? PrescriptionId { get; }
}
