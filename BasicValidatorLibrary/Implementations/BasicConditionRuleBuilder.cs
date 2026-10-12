using System.Linq.Expressions;
using BasicValidatorLibrary.Interfaces;
using BasicValidatorLibrary.Models;

namespace BasicValidatorLibrary.Implementations;

public class BasicConditionRuleBuilder<TProperty>(
    Expression<Func<TProperty, bool>> condition,
    Action action)
    : IConditionRuleBuilder<TProperty>, IProcessRunner<TProperty>
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

    public Task<ProcessReturnModel> ProcessAsync(TProperty value)
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
                    return Task.FromResult(new ProcessReturnModel
                    {
                        IsSuccess = false,
                        ErrorMessage = ex.Message
                    });
                }
                break;
            }
        }

        return Task.FromResult(new ProcessReturnModel
        {
            IsSuccess = true,
            ErrorMessage = string.Empty
        });
    }
}