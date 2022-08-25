namespace GrandfathersClock.Core.Logic;
public class CustomWaste : IWaste, ISerializable
{
    public bool IsEnabled { get; set; }
    public int CardsNeededToBegin { get; set; }
    public int HowManyPiles { get; set; }
    public event WastePileSelectedEventHandler? PileSelectedAsync;
    public event WasteDoubleClickEventHandler? DoubleClickAsync;
    public CustomWaste()
    {
        if (PileSelectedAsync is null || DoubleClickAsync is null)
        {

        }
    }
    public void AddSingleCard(int whichOne, SolitaireCard thisCard)
    {

    }
    public void ClearBoard(IDeckDict<SolitaireCard> thisCol)
    {

    }
    public void DoubleClickColumn(int index)
    {

    }
    public void FirstLoad(bool isKlondike, IDeckDict<SolitaireCard> cardList)
    {

    }
    public void FirstLoad(int rows, int columns)
    {

    }

    public void GetUnknowns()
    {

    }
    public async Task LoadGameAsync(SavedWaste gameData)
    {

        await Task.CompletedTask;
    }

    public void MoveCards(int whichOne, int lasts)
    {

    }
    public void MoveSingleCard(int whichOne)
    {

    }
    public void RemoveSingleCard()
    {

    }
    public void SelectUnselectPile(int whichOne)
    {

    }
    public void UnselectAllColumns()
    {

    }
    public int OneSelected()
    {
        throw new NotImplementedException();
    }
    public bool CanAddSingleCard(int WhichOne, SolitaireCard thisCard)
    {
        throw new NotImplementedException();
    }
    public bool CanMoveCards(int whichOne, out int lastOne)
    {
        throw new NotImplementedException();
    }
    public bool CanMoveToAnotherPile(int whichOne)
    {
        throw new NotImplementedException();
    }
    public bool CanSelectUnselectPile(int whichOne)
    {
        throw new NotImplementedException();
    }
    public IDeckDict<SolitaireCard> GetAllCards()
    {
        throw new NotImplementedException();
    }
    public SolitaireCard GetCard()
    {
        throw new NotImplementedException();
    }
    public SolitaireCard GetCard(int whichOne)
    {
        throw new NotImplementedException();
    }
    public Task<SavedWaste> GetSavedGameAsync()
    {
        throw new NotImplementedException();
    }
    public bool HasCard(int whichOne)
    {
        throw new NotImplementedException();
    }
}
