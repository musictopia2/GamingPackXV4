namespace BasicGameFrameworkLibrary.Core.SolitaireClasses.PileInterfaces;
public interface IMain
{
    int HowManyPiles();
    bool CanAddCard(int pile, SolitaireCard thisCard);
    void SetSavedScore(int score);
    void ClearBoard(IDeckDict<SolitaireCard> thisList);
    void ClearBoard();
    void AddCard(int pile, SolitaireCard thisCard);
    Func<int, Task>? PileSelectedAsync { get; set; }
    int CardsNeededToBegin { get; set; }
    void FirstLoad(bool needToMatch, bool showNextNeeded);
    bool IsRound { get; set; }
    int Rows { get; set; }
    int Columns { get; set; }
    Task LoadGameAsync(string data);
    Task<string> GetSavedPilesAsync();
    void AddCards(int Pile, IDeckDict<SolitaireCard> list);
    void AddCards(IDeckDict<SolitaireCard> list);
    bool HasCard(int pile);
    int StartNumber();
}