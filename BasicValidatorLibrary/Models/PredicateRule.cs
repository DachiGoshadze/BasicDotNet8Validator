using System.Linq.Expressions;

namespace BasicValidatorLibrary.Models;

public class PredicateRule<TProperty>
{
    public Expression<Func<TProperty, bool>>? Predicate { get; set; }
    public Expression<Func<TProperty, Task<bool>>>? PredicateAsync { get; set; }
    public string Message { get; set; } = "Unknown error";
}
