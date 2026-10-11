namespace BasicValidatorLibrary.Interfaces;

public interface IConditionRule<in TProperty>
{
    public Task Process(TProperty value);
}