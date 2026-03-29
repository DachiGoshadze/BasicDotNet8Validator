using System.Reflection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Controllers;

namespace BasicValidatorLibrary;

public class ValidatorMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        context.Request.EnableBuffering();

        using var reader = new StreamReader(
            context.Request.Body,
            leaveOpen: true);
        

        var body = await reader.ReadToEndAsync();

        Console.WriteLine($"Request Body: {body}");
        Console.WriteLine($"Request Path: {context.Request.Path}");
        var endpoint = context.GetEndpoint();

        var descriptor = endpoint?
            .Metadata
            .GetMetadata<ControllerActionDescriptor>();

        if (descriptor != null)
        {
            var parameters = descriptor.MethodInfo.GetParameters();

            foreach (var p in parameters)
            {
                if (!p.ParameterType.IsClass || p.ParameterType == typeof(string)) continue;
                var className = p.ParameterType.Name;
                // var validationResult = manager.Validate(JsonConvert.DeserializeObject<type>(className);
                // if (!validationResult)
                // {
                //     context.Response.StatusCode = StatusCodes.Status400BadRequest;
                //     await context.Response.WriteAsJsonAsync(new
                //     {
                //         Errors = validationResult.Errors.Select(e => new { e.PropertyName, e.ErrorMessage })
                //     });
                //     return;
                // }
            }
        }
        
        var methodInfo = endpoint?
            .Metadata
            .GetMetadata<MethodInfo>();

        if (methodInfo != null)
        {
            var parameters = methodInfo.GetParameters();

            foreach (var p in parameters)
            {
                Console.WriteLine($"{p.Name} : {p.ParameterType}");
            }
        }
        
        context.Request.Body.Position = 0;
        
        await next(context);
    }
}
