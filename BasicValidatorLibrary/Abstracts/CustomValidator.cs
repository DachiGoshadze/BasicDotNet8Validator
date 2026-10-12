using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using BasicValidatorLibrary.Implementations;
using BasicValidatorLibrary.Interfaces;

namespace BasicValidatorLibrary.Abstracts;

public abstract class CustomValidator<T> : IValidator
    where T : class
{
    public bool ForceStopIfFailed { get; set; } = true;
    private List<IProcessRunner<T>> Rules { get; set; } = [];

    protected IRuleBuilder<T, TProperty> RuleFor<TProperty>(Expression<Func<T, TProperty>> validationSelectExpression)
    {
        var ruleBuilder = new BasicRuleBuilder<T, TProperty>(validationSelectExpression);
        Rules.Add(ruleBuilder);
        return ruleBuilder;
    }

    protected IConditionRuleBuilder<T> When(Expression<Func<T, bool>> condition, Action action)
    {
        var conditionRuleBuilder = new BasicConditionRuleBuilder<T>(condition, action);

        Rules.Add(conditionRuleBuilder);
        return conditionRuleBuilder;
    }

    protected ICustomValidatorSetter<T, TProperty> ValidatorFor<TProperty>(Func<T, TProperty> selector,
        CustomValidator<TProperty> validator, ValidatorManager manager) where TProperty : class
    {
        var b = new BasicCustomValidatorSetter<T, TProperty>();
        b.SetValidator(selector, validator, manager);
        Rules.Add(b);
        return b;
    }

    public async Task<(bool, string)> ValidateAsync(T value)
    {
        foreach (var rule in Rules)
        {
            var result = await rule.ProcessAsync(value);
            if (!result.IsSuccess)
            {
                return (result.IsSuccess,
                    !string.IsNullOrEmpty(result.ErrorMessage) ? result.ErrorMessage : string.Empty);
            }
        }

        return (true, string.Empty);
    }

    public async Task ValidateAndThrowAsync(T value)
    {
        var msg = "";
        var failed = false;
        foreach (var rule in Rules)
        {
            var result = await rule.ProcessAsync(value);
            if (result.IsSuccess) continue;
            failed = true;
            if (ForceStopIfFailed)
                throw new ValidationException(!string.IsNullOrEmpty(result.ErrorMessage)
                    ? $"Validation failed: {result.ErrorMessage};"
                    : string.Empty);
            msg += !string.IsNullOrEmpty(result.ErrorMessage)
                ? "Validation failed: " + result.ErrorMessage
                : string.Empty;
        }

        if (failed) throw new ValidationException(msg);
    }

    public async Task BaseValidateAsync(object? value)
    {
        var castedValue = value as T;
        if (castedValue == null)
        {
            throw new InvalidOperationException($"Value must be of type {typeof(T).Name}");
        }

        await ValidateAndThrowAsync(castedValue);
    }
}