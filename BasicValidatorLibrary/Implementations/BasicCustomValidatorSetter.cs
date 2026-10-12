using BasicValidatorLibrary.Abstracts;
using BasicValidatorLibrary.Interfaces;
using BasicValidatorLibrary.Models;

namespace BasicValidatorLibrary.Implementations;

public class BasicCustomValidatorSetter<T, TProperty> : ICustomValidatorSetter<T, TProperty>, IProcessRunner<T> where T : class where TProperty : class
{
    private CustomValidator<TProperty>? _validator;
    private Func<T, TProperty?> _selector = T => T as TProperty;
    public void SetValidator(Func<T, TProperty?> selector, CustomValidator<TProperty>? validator, ValidatorManager validatorManager)
    {
        if(validatorManager.ExistsValidators(typeof(TProperty).Name))
        {
            throw new Exception($"Validator for type {typeof(TProperty).Name} not exists in the ValidatorManager.");
        }
        _validator = validator;
        _selector = selector;
    }

    public async Task<ProcessReturnModel> ProcessAsync(T value)
    {
        try
        {
            var selectRes = _selector(value);
            if (_validator is null || selectRes is null)
            {
                return new ProcessReturnModel
                {
                    IsSuccess = true,
                    ErrorMessage = string.Empty
                };
            }
            await _validator.ValidateAndThrowAsync(selectRes).ConfigureAwait(false);
            return new ProcessReturnModel
            {
                IsSuccess = true,
                ErrorMessage = string.Empty
            };
        }
        catch (Exception ex)
        {
            return new ProcessReturnModel
            {
                IsSuccess = false,
                ErrorMessage = ex.Message
            };
        }
    }
}