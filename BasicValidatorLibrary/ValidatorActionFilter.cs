using BasicValidatorLibrary.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace BasicValidatorLibrary;

public class ValidatorActionFilter(IOptions<BasicValidatorOptions> opt, ValidatorManager validatorManager)
    : IAsyncActionFilter
{
    private readonly Dictionary<string, Type> _validators = validatorManager.GetValidators();

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        foreach (var argument in context.ActionArguments)
        {
            if (argument.Value is null) continue;
            if (!_validators.ContainsKey(argument.Value.GetType().ToString()))
            {
                try
                {
                    await DeepValidation(argument.Value, context);
                    await next();
                    return;
                }
                catch (Exception ex)
                {
                    context.Result = new BadRequestObjectResult(new
                        { error = string.IsNullOrEmpty(ex.Message) ? opt.Value.DefaultErrorMessage : ex.Message });
                    return;
                }
            }

            var validator = (IValidator)context.HttpContext.RequestServices
                .GetRequiredService(_validators[argument.Value.GetType().ToString()]);
            try
            {
                await validator.ValidateAsync(argument.Value);
                await DeepValidation(argument.Value, context);
            }
            catch (Exception ex)
            {
                context.Result = new BadRequestObjectResult(new
                    { error = string.IsNullOrEmpty(ex.Message) ? opt.Value.DefaultErrorMessage : ex.Message });
                return;
            }
        }

        await next();
    }

    private async Task DeepValidation(object? value, ActionExecutingContext context)
    {
        if (value == null) return;
        var properties = value.GetType().GetProperties();
        foreach (var property in properties)
        {
            if (!property.PropertyType.IsClass) continue;
            var propertyValue = property.GetValue(value);
            if (!_validators.ContainsKey(property.PropertyType.ToString()))
            {
                await DeepValidation(propertyValue, context);
                continue;
            }

            var validator = (IValidator)context.HttpContext.RequestServices
                .GetRequiredService(_validators[property.PropertyType.ToString()]);
            await validator.ValidateAsync(propertyValue);
            await DeepValidation(propertyValue, context);
        }
    }
}