using System.Linq.Expressions;
using BasicValidatorLibrary.Implementations;
using BasicValidatorLibrary.Interfaces;

namespace BasicValidatorLibrary;

public abstract class CustomValidator<T>
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
                throw new Exception("Validation failed: " + result.errorMessage);
            }
        }
    }
}