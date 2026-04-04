using System.Reflection;
using BasicValidatorLibrary.Abstracts;
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

    public static void AddValidatorsFromAssembly(this IServiceCollection serviceCollection, Assembly assembly)
    {
        var types = assembly.GetTypes()
            .Where(t => !t.IsAbstract && IsSubclassOfRawGeneric(typeof(CustomValidator<>), t))
            .ToList();
        foreach (var type in types)
        {
            serviceCollection.AddTransient(type);
        }
    }
}