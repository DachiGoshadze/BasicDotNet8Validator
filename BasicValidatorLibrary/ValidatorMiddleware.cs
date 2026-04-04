// using System.Reflection;
// using BasicValidatorLibrary.Abstracts;
// using BasicValidatorLibrary.Interfaces;
// using Microsoft.AspNetCore.Http;
// using Microsoft.AspNetCore.Mvc;
// using Microsoft.AspNetCore.Mvc.Controllers;
// using Microsoft.Extensions.DependencyInjection;
// using Newtonsoft.Json;
// using Newtonsoft.Json.Linq;
//
// namespace BasicValidatorLibrary;
//
// public class ValidatorMiddleware(RequestDelegate next)
// {
//     public async Task InvokeAsync(HttpContext context, [FromServices] ValidatorManager validatorManager,
//         IServiceProvider serviceProvider)
//     {
//         try
//         {
//             context.Request.EnableBuffering();
//             var validators = validatorManager.GetValidators();
//             using var reader = new StreamReader(
//                 context.Request.Body,
//                 leaveOpen: true);
//             Console.WriteLine($"Request Path: {context.Request.Path}");
//             var endpoint = context.GetEndpoint();
//
//             var parameters = endpoint?
//                 .Metadata.GetMetadata<ControllerActionDescriptor>()!.MethodInfo.GetParameters() ?? 
//                  endpoint?.Metadata.GetMetadata<MethodInfo>()?.GetParameters();
//             if(parameters == null) throw new ArgumentNullException(nameof(parameters));
//             foreach (var p in parameters)
//             {
//                 if (!p.ParameterType.IsClass) continue;
//                 if (!validators.ContainsKey(p.ParameterType.Name)) continue;
//                 var attribute = p.CustomAttributes.LastOrDefault(x =>
//                     x.AttributeType == typeof(FromBodyAttribute) ||
//                     x.AttributeType == typeof(FromFormAttribute) ||
//                     x.AttributeType == typeof(FromQueryAttribute));
//                 if (attribute == null) continue;
//
//                 var paramType = validators[p.ParameterType.Name];
//                 var service = (IValidator)serviceProvider.GetRequiredService(paramType);
//
//                 switch (attribute.AttributeType.Name)
//                 {
//                     case nameof(FromBodyAttribute):
//
//                         var body = await reader.ReadToEndAsync();
//                         var json = JObject.Parse(body);
//                         if (p.Name != null)
//                         {
//                             var parameterValue = json[p.Name]?.ToString();
//                             var des = JsonConvert.DeserializeObject(parameterValue ?? body, p.ParameterType);
//                             if (des == null) continue;
//                             await service.ValidateAsync(des);
//                         }
//
//                         context.Request.Body.Position = 0;
//                         break;
//                     case nameof(FromFormAttribute):
//                         break;
//                     case nameof(FromQueryAttribute):
//                         var newInstance = Activator.CreateInstance(p.ParameterType);
//                         break;
//                 }
//             }
//         }
//         catch (Exception ex)
//         {
//             Console.WriteLine($"Error in ValidatorMiddleware: {ex.Message}");
//         }
//
//         context.Request.Body.Position = 0;
//
//         await next(context);
//     }
// }