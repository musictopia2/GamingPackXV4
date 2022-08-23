namespace BasicGameFrameworkLibrary.Core.MultiplayerClasses.BasicPlayerClasses;

public interface IPlayerObject<D> : IPlayerItem where D : IDeckObject, new()
{
    DeckRegularDict<D> MainHandList { get; set; }
    DeckRegularDict<D> StartUpList { get; set; }
}