using BasicValidatorLibrary.Abstracts;

namespace BasicValidatorLibrary.Interfaces;

public interface ICustomValidatorSetter<T, TProperty> where T : class where TProperty : class
{
    public void SetValidator(Func<T, TProperty?> selector, CustomValidator<TProperty>? validator, ValidatorManager validatorManager);
}