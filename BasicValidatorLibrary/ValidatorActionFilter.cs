using BasicValidatorLibrary.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;

namespace BasicValidatorLibrary;

public class ValidatorActionFilter(ValidatorManager validatorManager) : IAsyncActionFilter
{
    private readonly Dictionary<string, Type> validators = validatorManager.GetValidators();
    
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        foreach (var argument in context.ActionArguments)
        {
            if(argument.Value is null) continue;
            if (!validators.ContainsKey(argument.Value.GetType().ToString()))
            {
                try
                {
                    await DeepValidation(argument.Value, context);
                    await next();
                    return;
                } catch (Exception ex)
                {
                    context.Result = new Microsoft.AspNetCore.Mvc.BadRequestObjectResult(new { error = ex.Message });
                    return;
                }
            }
            var validator = (IValidator)context.HttpContext.RequestServices
                .GetRequiredService(validators[argument.Value.GetType().ToString()]);
            try
            {
                await validator.ValidateAsync(argument.Value);
                await DeepValidation(argument.Value, context);
            }
            catch (Exception ex)
            {
                context.Result = new Microsoft.AspNetCore.Mvc.BadRequestObjectResult(new { error = ex.Message });
                return;
            }
        }

        await next();
    }

    private async Task DeepValidation(object? value, ActionExecutingContext context) 
    {
        if(value == null) return;
        var properties = value.GetType().GetProperties();
        foreach (var property in properties)
        {
            if(!property.PropertyType.IsClass) continue;
            var propertyValue = property.GetValue(value);
            if (!validators.ContainsKey(property.PropertyType.ToString()))
            {
                await DeepValidation(propertyValue, context);
                continue;
            }
            var validator = (IValidator)context.HttpContext.RequestServices
                .GetRequiredService(validators[property.PropertyType.ToString()]);
            await validator.ValidateAsync(propertyValue);
            await DeepValidation(propertyValue, context);
        }
    }
}