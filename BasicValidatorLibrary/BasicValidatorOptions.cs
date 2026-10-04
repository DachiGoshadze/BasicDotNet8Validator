using System.Reflection;
using Microsoft.AspNetCore.Mvc;

namespace BasicValidatorLibrary;

public class BasicValidatorOptions
{
    public Assembly Assembly { get; set; } = Assembly.GetCallingAssembly();
    public bool UseGlobalControllerValidation { get; set; } 
    public string DefaultErrorMessage { get; set; } = "Request Parameters Validation Failed";
}
