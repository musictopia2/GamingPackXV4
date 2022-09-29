namespace CaptiveQueensSolitaire.Core.Logic;
public class CustomWaste : IWaste, ISerializable
{
    public bool IsEnabled { get; set; }
    public int CardsNeededToBegin { get; set; }
    public int HowManyPiles { get; set; }
    public Func<int, Task>? PileSelectedAsync { get; set; }
    public Func<int, Task>? DoubleClickAsync { get; set; }
    public CustomWaste()
    {
        if (PileSelectedAsync is null || DoubleClickAsync is null)
        {

        }
    }
    public void AddSingleCard(int whichOne, SolitaireCard thisCard)
    {

    }
    public DeckRegularDict<SolitaireCard> CardList = new();
    public void ClearBoard(IDeckDict<SolitaireCard> thisCol)
    {
        if (thisCol.Count != CardsNeededToBegin)
        {
            throw new CustomBasicException($"Needs {CardsNeededToBegin}, not {thisCol.Count}");
        }
        if (thisCol.Any(items => items.Value != EnumRegularCardValueList.Queen))
        {
            throw new CustomBasicException("Only queens can be used");
        }
        thisCol.First().Rotated = true;
        thisCol[2].Rotated = true;
        CardList.ReplaceRange(thisCol);
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

        DeckRegularDict<SolitaireCard> tempList = await js.DeserializeObjectAsync<DeckRegularDict<SolitaireCard>>(gameData.PileData);
        CardList.ReplaceRange(tempList);
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
        return -1;
    }
    public bool CanAddSingleCard(int WhichOne, SolitaireCard thisCard)
    {
        return false;
    }

    public bool CanMoveCards(int whichOne, out int lastOne)
    {
        lastOne = -1;
        return false;
    }

    public bool CanMoveToAnotherPile(int whichOne)
    {
        return false;
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

    public async Task<SavedWaste> GetSavedGameAsync()
    {
        SavedWaste output = new();
        output.PileData = await js.SerializeObjectAsync(CardList.ToRegularDeckDict());
        return output;
    }
    public bool HasCard(int whichOne)
    {
        return false;
    }
}