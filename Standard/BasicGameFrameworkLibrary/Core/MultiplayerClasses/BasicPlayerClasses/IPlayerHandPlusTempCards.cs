namespace BasicGameFrameworkLibrary.Core.MultiplayerClasses.BasicPlayerClasses;
public interface IPlayerHandPlusTempCards<D> : IPlayerSingleHand<D>
    where D : IDeckObject, new()
{
    DeckRegularDict<D> AdditionalObjects { get; set; }
}