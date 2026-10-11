using System.Linq.Expressions;

namespace BasicValidatorLibrary.Models;

public class ConditionView<TProperty>
{
    public Expression<Func<TProperty, bool>>? Condition { get; set; } = null;
    public Action? Action { get; set; } = null;
}