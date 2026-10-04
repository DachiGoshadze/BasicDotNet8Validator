namespace BasicValidatorLibrary.Interfaces;

public interface IValidator
{
    public Task ValidateAsync(object? value);
}