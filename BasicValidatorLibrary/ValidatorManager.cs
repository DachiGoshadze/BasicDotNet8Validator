namespace BasicValidatorLibrary;

public class ValidatorManager
{
    private Dictionary<string, Type> Validators { get; set; } = new();
    
    public void AddValidator(string key, Type type)
    {
        Validators[key] = type;
    }
    public Dictionary<string, Type> GetValidators() => Validators;
}