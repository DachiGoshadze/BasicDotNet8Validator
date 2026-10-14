using BasicValidatorLibrary.Abstracts;

namespace BasicValidatorLibrary.Interfaces;

public interface ICustomValidatorSetter<TProperty> where TProperty : class
{
    public void SetValidator(CustomValidator<TProperty>? validator, ValidatorManager validatorManager);
}