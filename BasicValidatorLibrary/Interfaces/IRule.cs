namespace BasicValidatorLibrary.Interfaces;

public interface IRule<in T>
{
    public  Task<(bool isValid, string errorMessage)> ProcessValidateAsync(T value);
}