using BasicValidatorLibrary.Abstracts;
using BasicValidatorLibrary.Interfaces;
using BasicValidatorLibrary.Models;
using Microsoft.AspNetCore.Mvc;

namespace BasicValidatorLibrary.Implementations;

public class MultiCustomValidatorSetter<T, TProperty>(Func<T, IEnumerable<TProperty?>> selector)
    : ICustomValidatorSetter<TProperty>, IProcessRunner<T>
    where T : class
    where TProperty : class
{
    private CustomValidator<TProperty>? _validator;

    public void SetValidator(CustomValidator<TProperty>? validator, [FromServices] ValidatorManager validatorManager)
    {
        if (validatorManager.ExistsValidators(typeof(TProperty).Name))
        {
            throw new Exception($"Validator for type {typeof(TProperty).Name} not exists in the ValidatorManager.");
        }

        _validator = validator;
    }

    public async Task<ProcessReturnModel> ProcessAsync(T value)
    {
        try
        {
            var selectRes = selector(value);
            if (_validator is null)
            {
                return new ProcessReturnModel
                {
                    IsSuccess = true,
                    ErrorMessage = "validator is null, skipping validation."
                };
            }

            foreach (var prop in selectRes)
            {
                if (prop is null)
                {
                    return new ProcessReturnModel
                    {
                        IsSuccess = false,
                        ErrorMessage = "Property is null."
                    };
                }
                await _validator.ValidateAndThrowAsync(prop).ConfigureAwait(false);
            }
            
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