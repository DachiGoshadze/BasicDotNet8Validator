using System.Reflection;

namespace BasicValidatorLibrary;

public class BasicValidatorOptions
{
    public Assembly Assembly { get; set; }
    public bool UseGlobalControllerValidation { get; set; }
}
