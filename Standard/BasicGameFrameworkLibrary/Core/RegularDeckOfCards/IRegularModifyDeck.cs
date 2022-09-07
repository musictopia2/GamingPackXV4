namespace BasicGameFrameworkLibrary.Core.RegularDeckOfCards;
public interface IRegularModifyDeck
{
    void RemoveSuit(EnumSuitList suitToRemove);
    void ReloadSuits();
}