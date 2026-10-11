using System.Linq.Expressions;
using BasicValidatorLibrary.Interfaces;
using BasicValidatorLibrary.Models;

namespace BasicValidatorLibrary.Implementations;

public class BasicConditionRuleBuilder<TProperty>(
    Expression<Func<TProperty, bool>> condition,
    Action action)
    : IConditionRuleBuilder<TProperty>, IRule<TProperty>
{
    private List<ConditionView<TProperty>> Conditions { get; set; } =
        [new() { Condition = condition, Action = action }];

    public Task Process(TProperty value)
    {
        foreach (var cond in Conditions)
        {
            if (cond.Condition == null)
            {
                if (cond.Action == null) continue;
                cond.Action();
            }

            if (cond.Condition?.Compile().Invoke(value) ?? true)
            {
                try
                {
                    cond.Action?.Invoke();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error executing action: {ex.Message}");
                }
                break;
            }
        }

        return Task.CompletedTask;
    }

    public IConditionRuleBuilder<TProperty> Otherwise(
        Action action, Expression<Func<TProperty, bool>>? condition = null)
    {
        Conditions.Add(new ConditionView<TProperty> { Condition = condition, Action = action });
        return this;
    }

    public Task<(bool isValid, string errorMessage)> ProcessValidateAsync(TProperty value)
    {
        foreach (var cond in Conditions)
        {
            if (cond.Condition == null)
            {
                if (cond.Action == null) continue;
                cond.Action();
            }

            if (cond.Condition?.Compile().Invoke(value) ?? true)
            {
                try
                {
                    cond.Action?.Invoke();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error executing action: {ex.Message}");
                }
                break;
            }
        }

        return Task.FromResult((true, ""));
    }
}