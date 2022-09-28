namespace BasicGameFrameworkLibrary.Core.SolitaireClasses.PileInterfaces;
public interface IWaste
{
    void FirstLoad(bool isKlondike, IDeckDict<SolitaireCard> cardList);
    void FirstLoad(int rows, int columns);
    void ClearBoard(IDeckDict<SolitaireCard> thisCol);
    void GetUnknowns();
    bool CanAddSingleCard(int WhichOne, SolitaireCard thisCard);
    int CardsNeededToBegin { get; set; }
    bool CanMoveCards(int whichOne, out int lastOne);
    void MoveCards(int whichOne, int lasts);
    bool CanMoveToAnotherPile(int whichOne);
    void MoveSingleCard(int whichOne);
    IDeckDict<SolitaireCard> GetAllCards();
    Func<int, Task>? PileSelectedAsync { get; set; }
    Func<int, Task>? DoubleClickAsync { get; set; }
    int HowManyPiles { get; set; }
    Task<SavedWaste> GetSavedGameAsync();
    Task LoadGameAsync(SavedWaste gameData);
    int OneSelected();
    void UnselectAllColumns();
    void SelectUnselectPile(int whichOne);
    bool HasCard(int whichOne);
    SolitaireCard GetCard();
    bool CanSelectUnselectPile(int whichOne);
    void DoubleClickColumn(int index);
    void RemoveSingleCard();
    void AddSingleCard(int whichOne, SolitaireCard thisCard);
    SolitaireCard GetCard(int whichOne);
}