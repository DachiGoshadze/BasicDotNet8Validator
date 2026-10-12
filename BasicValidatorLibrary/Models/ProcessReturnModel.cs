namespace BasicValidatorLibrary.Models;

public class ProcessReturnModel
{
    public bool IsSuccess { get; set; } = false;
    public string ErrorMessage { get; set; } = string.Empty;
}