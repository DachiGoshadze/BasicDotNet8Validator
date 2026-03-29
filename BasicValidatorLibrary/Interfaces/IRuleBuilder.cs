using System.Linq.Expressions;

namespace BasicValidatorLibrary.Interfaces;

public interface IRuleBuilder<T, TProperty>
{
    public Expression<Func<T, TProperty>> SelectedModelFunc { get; set; }
    public IRuleBuilder<T, TProperty> MustBe(Expression<Func<TProperty, bool>> predicate);
    public IRuleBuilder<T, TProperty> WithMessage(string message);
    public IRuleBuilder<T, TProperty> NotEmpty();
    public IRuleBuilder<T, TProperty> NotNull();
    public IRuleBuilder<T, TProperty> NotNullOrEmpty();
    public IRuleBuilder<T, TProperty> MaximumLength(int value);
    public IRuleBuilder<T, TProperty> MinimumLength(int value);
    public IRuleBuilder<T, TProperty> GreaterThen(int value);
    public IRuleBuilder<T, TProperty> LessThen(int value);
    public IRuleBuilder<T, TProperty> GreaterThenOrEqual(int value);
    public IRuleBuilder<T, TProperty> LessThenOrEqual(int value);
    public IRuleBuilder<T, TProperty> Equal(int value);
    public IRuleBuilder<T, TProperty> Equal(string value);
    public IRuleBuilder<T, TProperty> Contains(string value);
    public IRuleBuilder<T, TProperty> Email();
    public IRuleBuilder<T, TProperty> Url();
}