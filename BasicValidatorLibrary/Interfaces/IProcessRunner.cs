using BasicValidatorLibrary.Models;

namespace BasicValidatorLibrary.Interfaces;

public interface IProcessRunner<in T>
{
    public  Task<ProcessReturnModel> ProcessAsync(T value);
}