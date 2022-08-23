namespace BasicGameFrameworkLibrary.Core.BasicDrawables.Interfaces;

public interface IRummmyObject<S, C> : ISimpleValueObject<int>, IWildObject,
   IIgnoreObject, ISuitObject<S>, IColorObject<C>
   where S : IFastEnumSimple where C : IFastEnumColorSimple
{
    int GetSecondNumber { get; } //you should not be able to change it.
}