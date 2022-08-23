namespace BasicGameFrameworkLibrary.Core.MultiplayerClasses.SavedGameClasses;
public interface ISavedCardList<D> where D : class, IDeckObject, new()
{
    DeckRegularDict<D> CardList { get; set; } //try to make it not nullable anymore (?)
    D? CurrentCard { get; set; }
}