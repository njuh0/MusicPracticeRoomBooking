using System.ComponentModel.DataAnnotations;

namespace WebApp.Validation;

public class GreaterThanAttribute : ValidationAttribute
{
    private readonly string _comparisonProperty;

    public GreaterThanAttribute(string comparisonProperty)
    {
        _comparisonProperty = comparisonProperty;
        ErrorMessage = "{0} must be greater than {1}.";
    }

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        var currentValue = value as IComparable;
        var property = validationContext.ObjectType.GetProperty(_comparisonProperty);

        if (property == null)
        {
            return new ValidationResult($"Unknown property: {_comparisonProperty}");
        }

        var comparisonValue = property.GetValue(validationContext.ObjectInstance) as IComparable;

        if (currentValue == null || comparisonValue == null)
        {
            return ValidationResult.Success;
        }

        if (currentValue.CompareTo(comparisonValue) <= 0)
        {
            return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
        }

        return ValidationResult.Success;
    }

    public override string FormatErrorMessage(string name)
    {
        return string.Format(ErrorMessageString, name, _comparisonProperty);
    }
}
