using System.Linq.Expressions;
using BasicValidatorLibrary.Implementations;
using BasicValidatorLibrary.Interfaces;

namespace BasicValidatorLibrary.Abstracts;

public abstract class CustomValidator<T> : IValidator 
    where T : class 
{
    private List<IRule<T>> Rules { get; set; } = [];

    protected IRuleBuilder<T, TProperty> RuleFor<TProperty>(Expression<Func<T, TProperty>> validationSelectExpression)
    {
        var ruleBuilder = new BasicRuleBuilder<T, TProperty>(validationSelectExpression);
        Rules.Add(ruleBuilder);
        return ruleBuilder;
    }
    public async Task<(bool, string)> ValidateAsync(T value)
    {
        foreach (var rule in Rules)
        {
            var result = await rule.ProcessValidateAsync(value);
            if (!result.isValid)
            {
                return (result.isValid, result.errorMessage);
            }
        }
        return (true, string.Empty);
    }
    
    public async Task ValidateAndThrowAsync(T value)
    {
        foreach (var rule in Rules)
        {
            var result = await rule.ProcessValidateAsync(value);
            if (!result.isValid)
            {
                throw new Exception(string.IsNullOrEmpty(result.errorMessage) ? "Validation failed: " + result.errorMessage : string.Empty);
            }
        }
    }

    public async Task ValidateAsync(object? value)
    {
        var castedValue = value as T;
        if (castedValue == null)
        {
            throw new InvalidOperationException($"Value must be of type {typeof(T).Name}");
        }
        await ValidateAndThrowAsync(castedValue);
    }
}