using System.Linq.Expressions;
using System.Text.RegularExpressions;
using BasicValidatorLibrary.Interfaces;
using BasicValidatorLibrary.Models;

namespace BasicValidatorLibrary.Implementations;

public class BasicRuleBuilder<T, TProperty>(Expression<Func<T, TProperty>> selectedModelFunc)
    : IRuleBuilder<T, TProperty>, IRule<T>
{
    private List<PredicateRule<TProperty>> ValidationRules { get; } = new();

    public Expression<Func<T, TProperty>> SelectedModelFunc { get; set; } = selectedModelFunc;

    public IRuleBuilder<T, TProperty> MustBe(Expression<Func<TProperty, bool>> predicate)
    {
        ValidationRules.Add(new PredicateRule<TProperty> { Predicate = predicate, Message = string.Empty });
        return this;
    }

    public IRuleBuilder<T, TProperty> MustBeAsync(Expression<Func<TProperty, Task<bool>>> predicate)
    {
        ValidationRules.Add(new PredicateRule<TProperty> { PredicateAsync = predicate, Message = string.Empty });
        return this;
    }

    public IRuleBuilder<T, TProperty> WithMessage(string message)
    {
        ValidationRules.Last().Message = message;
        return this;
    }

    public IRuleBuilder<T, TProperty> NotEmpty()
    {
        if (typeof(TProperty) != typeof(string))
            throw new InvalidOperationException("NotEmpty can only be used with string properties");
        return MustBe(x => x as string != "");
    }

    public IRuleBuilder<T, TProperty> NotNull()
    {
        return MustBe(x => x != null);
    }

    public IRuleBuilder<T, TProperty> NotNullOrEmpty()
    {
        if (typeof(TProperty) != typeof(string))
            throw new InvalidOperationException("NotNullOrEmpty can only be used with string properties");
        return MustBe(x => !string.IsNullOrEmpty(x as string));
    }

    public IRuleBuilder<T, TProperty> MaximumLength(int value)
    {
        if (typeof(TProperty) != typeof(string))
            throw new InvalidOperationException("MaximumLength can only be used with string properties");
        return MustBe(x => !string.IsNullOrEmpty(x as string));
    }

    public IRuleBuilder<T, TProperty> MinimumLength(int value)
    {
        if (typeof(TProperty) != typeof(string))
            throw new InvalidOperationException("MinimumLength can only be used with string properties");
        return MustBe(x => !string.IsNullOrEmpty(x as string));
    }

    public IRuleBuilder<T, TProperty> GreaterThen(int value)
    {
        if (typeof(TProperty) != typeof(int))
            throw new InvalidOperationException("GreaterThen can only be used with int properties");
        return MustBe(x => (int)(object)x! > value);
    }

    public IRuleBuilder<T, TProperty> LessThen(int value)
    {
        if (typeof(TProperty) != typeof(int))
            throw new InvalidOperationException("LessThen can only be used with int properties");
        return MustBe(x => (int)(object)x! < value);
    }

    public IRuleBuilder<T, TProperty> GreaterThenOrEqual(int value)
    {
        if (typeof(TProperty) != typeof(int))
            throw new InvalidOperationException("GreaterThenOrEqual can only be used with int properties");
        return MustBe(x => (int)(object)x! >= value);
    }

    public IRuleBuilder<T, TProperty> LessThenOrEqual(int value)
    {
        if (typeof(TProperty) != typeof(int))
            throw new InvalidOperationException("LessThenOrEqual can only be used with int properties");
        return MustBe(x => (int)(object)x! <= value);
    }

    public IRuleBuilder<T, TProperty> Equal(int value)
    {
        if (typeof(TProperty) != typeof(int))
            throw new InvalidOperationException("Equal can only be used with string or int properties");
        return MustBe(x => (int)(object)x! == value);
    }

    public IRuleBuilder<T, TProperty> Equal(string value)
    {
        if (typeof(TProperty) != typeof(string))
            throw new InvalidOperationException("Equal can only be used with string or int properties");
        return MustBe(x => x as string == value);
    }

    public IRuleBuilder<T, TProperty> Contains(string value)
    {
        if (typeof(TProperty) != typeof(string))
            throw new InvalidOperationException("Contains can only be used with string properties");
        return MustBe(x => x != null && x is string && (x as string)!.Contains(value));
    }

    public IRuleBuilder<T, TProperty> Email()
    {
        if (typeof(TProperty) != typeof(string))
            throw new InvalidOperationException("Email can only be used with string properties");
        return MustBe(x => x != null && x is string && Regex.IsMatch((x as string)!, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"));
    }

    public IRuleBuilder<T, TProperty> Url()
    {
        if (typeof(TProperty) != typeof(string))
            throw new InvalidOperationException("Url can only be used with string properties");
        return MustBe(x => x != null && x is string && Uri.IsWellFormedUriString(x as string, UriKind.Absolute));
    }

    public async Task<(bool isValid, string errorMessage)> ProcessValidateAsync(T value)
    {
        var propertyFunc = SelectedModelFunc.Compile();
        var propertyValue = propertyFunc(value);
        foreach (var rule in ValidationRules)
        {
            if (rule.Predicate != null)
            {
                var validate = rule.Predicate.Compile();
                if (!validate(propertyValue)) return (false, rule.Message);
            }
            else if (rule.PredicateAsync != null)
            {
                var validateAsync = rule.PredicateAsync.Compile();
                if (!await validateAsync(propertyValue)) return (false, rule.Message);
            }
        }

        return (true, string.Empty);
    }
}