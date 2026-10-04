using System.Reflection;
using BasicValidatorLibrary.Abstracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace BasicValidatorLibrary;

public static class ServiceExtension
{
    private static bool IsSubclassOfRawGeneric(Type generic, Type? toCheck)
    {
        if(toCheck == null) return false;
        
        toCheck = toCheck.BaseType;
        
        if(toCheck == null) return false;
        
        var cur = toCheck.IsGenericType
            ? toCheck.GetGenericTypeDefinition()
            : toCheck;

        return cur == generic;
    }
    
    public static void AddValidatorsFromAssembly(this IServiceCollection serviceCollection, BasicValidatorOptions opt)
    {
        var types = opt.Assembly.GetTypes()
            .Where(t => !t.IsAbstract && IsSubclassOfRawGeneric(typeof(CustomValidator<>), t))
            .ToList();
        var validationManager = new ValidatorManager();
        foreach (var type in types)
        {
            validationManager.AddValidator(type.BaseType!.GetGenericArguments()[0].ToString(), type);
            serviceCollection.AddTransient(type);
        }

        serviceCollection.AddSingleton(validationManager);
        if (opt.UseGlobalControllerValidation)
        {
            serviceCollection.Configure<MvcOptions>(options => { options.Filters.Add<ValidatorActionFilter>(); });
        }
    }
}