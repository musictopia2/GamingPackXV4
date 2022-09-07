namespace BasicGameFrameworkLibrary.Core.BasicDrawables.Interfaces;
public interface ITrickCard<S> : IDeckObject, IPlayerID, ILocation, //it needs to have something to know whether its wild.
    ISimpleValueObject<int>, ISuitObject<S>, IPointsObject, IWildObject where S : IFastEnumSimple
{
    object CloneCard(); //this is one where we do need to clone.
}