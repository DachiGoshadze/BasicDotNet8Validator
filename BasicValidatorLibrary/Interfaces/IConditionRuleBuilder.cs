using System.Linq.Expressions;

namespace BasicValidatorLibrary.Interfaces;

public interface IConditionRuleBuilder<TProperty>
{
    public IConditionRuleBuilder<TProperty> Otherwise(Action action, Expression<Func<TProperty, bool>>? conditio = null);
}