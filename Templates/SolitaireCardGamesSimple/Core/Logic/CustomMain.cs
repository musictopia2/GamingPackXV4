namespace SolitaireCardGamesSimple.Core.Logic;
public class CustomMain : IMain, ISerializable //good news is if nothing is found, just will do nothing.
{
    public int CardsNeededToBegin { get; set; }
    public bool IsEnabled { get; set; }
    public bool IsRound { get; set; }
    public int Rows { get; set; }
    public int Columns { get; set; }
    public event MainPileClickedEventHandler? PileSelectedAsync;
    public CustomMain() //only used to stop the warnings.
    {
        if (PileSelectedAsync is null)
        {

        }
    }
    public void SetSavedScore(int previousScore)
    {

    }
    public void ClearBoard()
    {
    }
    public void AddCard(int pile, SolitaireCard thisCard)
    {
    }
    public void FirstLoad(bool needToMatch, bool showNextNeeded)
    {
    }
    public int HowManyPiles()
    {
        return 0; // has to change to however many piles there are
    }
    public bool CanAddCard(int pile, SolitaireCard thiscard)
    {
        return false;
    }
    public bool HasCard(int Pile)
    {
        return false;
    }
    public int StartNumber()
    {
        throw new NotImplementedException(); // until i decide what to do
    }
    public void ClearBoard(IDeckDict<SolitaireCard> thisList)
    {

    }
    public async Task LoadGameAsync(string data)
    {
        await Task.CompletedTask;
    }
    public async Task<string> GetSavedPilesAsync()
    {
        return await Task.FromResult(""); //until i decide what the data will be (?)
    }
    public void AddCards(int Pile, IDeckDict<SolitaireCard> list)
    {

    }
    public void AddCards(IDeckDict<SolitaireCard> list)
    {

    }
}