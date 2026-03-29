using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace BasicValidatorLibrary;

public static class ServiceExtension
{
    private static bool IsSubclassOfRawGeneric(Type generic, Type? toCheck)
    {
        while (toCheck != null && toCheck != typeof(object))
        {
            var cur = toCheck.IsGenericType 
                ? toCheck.GetGenericTypeDefinition() 
                : toCheck;

            if (cur == generic)
                return true;

            toCheck = toCheck.BaseType;
        }
        return false;
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