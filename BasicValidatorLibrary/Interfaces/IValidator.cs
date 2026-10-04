namespace BasicValidatorLibrary.Interfaces;

public interface IValidator
{
    public Task BaseValidateAsync(object? value);
}