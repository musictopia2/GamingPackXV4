namespace BasicGameFrameworkLibrary.Core.DrawableListsObservable;
public partial class PlayerBoardObservable<TR> : SimpleControlObservable
    where TR : RegularTrickCard, new()
{
    private EnumPlayerBoardGameList _game = EnumPlayerBoardGameList.None;
    public EnumPlayerBoardGameList Game
    {
        get
        {
            return _game;
        }
        set
        {
            if (SetProperty(ref _game, value) == true)
            {
                if (HowManyRows == 0)
                {
                    if (Game == EnumPlayerBoardGameList.HorseShoe)
                    {
                        HowManyRows = 2;
                    }
                    else
                    {
                        HowManyRows = 4;
                    }
                }
            }
        }
    }
    public bool IsSelf { get; set; }
    public int HowManyRows { get; private set; }
    private const int _columns = 4;
    public DeckRegularDict<TR> CardList = new();
    public Action? SelectedCard { get; set; }
    public ControlCommand? CardCommand { get; set; }
    [Command(EnumCommandCategory.Control)]
#pragma warning disable IDE0051 // Remove unused private members cannot remove because part of source generators
    private void CardClick(TR card)
#pragma warning restore IDE0051 // Remove unused private members
    {
        if (IsSelf == false || card.IsEnabled == false || card.Visible == false)
        {
            return;
        }
        if (card.IsSelected == true)
        {
            card.IsSelected = false;
            return;
        }
        CardList.UnselectAllObjects();
        card.IsSelected = true;
        SelectedCard?.Invoke();
    }
    public PlayerBoardObservable(CommandContainer command) : base(command)
    {
        CreateCommands();
    }
    partial void CreateCommands();
    public void UnselectAllCards()
    {
        CardList.UnselectAllObjects();
    }
    protected override void EnableChange()
    {
        CardCommand!.ReportCanExecuteChange();
    }
    protected override void PrivateEnableAlways() { }
    public void ClearBoard(IDeckDict<TR> thisList)
    {
        if (Game == EnumPlayerBoardGameList.None)
        {
            throw new CustomBasicException("Must choose game");
        }
        else if (Game == EnumPlayerBoardGameList.Skuck && thisList.Count != 16)
        {
            throw new CustomBasicException("Skuck requires 16 cards");
        }
        else if (Game == EnumPlayerBoardGameList.HorseShoe && thisList.Count != 8)
        {
            throw new CustomBasicException("Horseshoe requires 8 cards");
        }
        CardList.ReplaceRange(thisList);
        RepositionCards();
    }
    private void RepositionCards()
    {
        foreach (var thisCard in CardList)
        {
            var (row, _) = GetRowColumnData(thisCard);
            thisCard.IsEnabled = false;
            if (Game == EnumPlayerBoardGameList.HorseShoe)
            {
                if (IsSelf == true & row == 2)
                {
                    thisCard.IsUnknown = false;
                    thisCard.IsEnabled = true;
                }
                else if (row == 1 & IsSelf == false)
                {
                    thisCard.IsUnknown = false;
                    thisCard.IsEnabled = true;
                }
                else
                {
                    thisCard.IsUnknown = true;
                }
            }
            else if (Game == EnumPlayerBoardGameList.Skuck)
            {
                if (IsSelf == true & row == 1)
                {
                    thisCard.IsUnknown = false;
                    thisCard.IsEnabled = true;
                }
                else if (IsSelf == true & row == 4)
                {
                    thisCard.IsUnknown = false;
                }
                else if (IsSelf == true)
                {
                    thisCard.IsUnknown = true;
                }
                else if (row == 1 | row == 4)
                {
                    thisCard.IsUnknown = false;
                    if (row == 4)
                    {
                        thisCard.IsEnabled = true;
                    }
                }
                else
                {
                    thisCard.IsUnknown = true;
                }
            }
        }
    }
    public (int row, int column) GetRowColumnData(TR thisCard)
    {
        int x;
        int y;
        int z;
        int p = 0;
        bool _isMinus;
        if (IsSelf == true & Game == EnumPlayerBoardGameList.Skuck | IsSelf == false & Game == EnumPlayerBoardGameList.HorseShoe)
        {
            p = HowManyRows * 2;
            z = HowManyRows + 1;
            _isMinus = true;
        }
        else
        {
            _isMinus = false;
            z = 0;
        }
        for (x = 1; x <= _columns; x++)
        {
            var loopTo = HowManyRows;
            for (y = 1; y <= loopTo; y++)
            {
                if (_isMinus == false)
                {
                    z += 1;
                }
                else
                {
                    z -= 1;
                }
                var tempCard = CardList[z - 1];
                if (tempCard.Deck == thisCard.Deck)
                {
                    return (y, x);
                }
            }
            if (_isMinus == true)
            {
                z += p;
            }
        }
        throw new CustomBasicException("Can't find row/column data for Card With Deck Of " + thisCard.Deck);
    }
    public void HideCard(TR thisCard)
    {
        if (thisCard.Visible == false)
        {
            throw new CustomBasicException("This card was already invisible.  Should not allow to click on it");
        }
        int oldVisible = CardList.Count(Items => Items.Visible == false);
        thisCard.Visible = false;
        thisCard.IsEnabled = false;
        thisCard.IsSelected = false;
        var nums = NextCard(thisCard);
        if (nums > -1)
        {
            var newCard = CardList[nums];
            if (newCard.Deck == thisCard.Deck)
            {
                throw new CustomBasicException("Can't be the same card");
            }
            newCard.IsUnknown = false;
            newCard.IsEnabled = true;
        };
        int newVisible = CardList.Count(Items => Items.Visible == false);
        if (oldVisible + 1 != newVisible)
        {
            throw new CustomBasicException("Only one card should have been invisible after hiding card.  Find out what happened");
        }
    }
    public TR GetCardByRowColumn(int row, int column)
    {
        foreach (var thisCard in CardList)
        {
            var (trow, tcolumn) = GetRowColumnData(thisCard);
            if (tcolumn == column && trow == row)
            {
                return thisCard;
            }
        }
        throw new CustomBasicException("No Card Found For Row " + row + " Column " + column);
    }
    private int FindSpecificCard(int row, int column)
    {
        foreach (var thisCard in CardList)
        {
            var (trow, tcolumn) = GetRowColumnData(thisCard);
            if (tcolumn == column && trow == row)
            {
                return CardList.IndexOf(thisCard);
            }
        }
        return -1;
    }
    private int NextCard(TR oldCard)
    {
        var (row, column) = GetRowColumnData(oldCard);
        bool isEnd;
        if (IsSelf == true)
        {
            if (Game == EnumPlayerBoardGameList.Skuck)
            {
                isEnd = true;
            }
            else
            {
                isEnd = false;
            }
        }
        else if (Game == EnumPlayerBoardGameList.HorseShoe)
        {
            isEnd = true;
        }
        else
        {
            isEnd = false;
        }

        if (isEnd == true && row == HowManyRows)
        {
            return -1;
        }

        if (isEnd == false && row == 1)
        {
            return -1;
        }
        int newRow;
        newRow = row;
        if (isEnd == true)
        {
            newRow += 1;
        }
        else
        {
            newRow -= 1;
        }

        return FindSpecificCard(newRow, column);
    }
    public bool IsFinished => CardList.All(items => items.Visible == false); //hopefully this way works too.
    public DeckRegularDict<TR> ValidCardList => CardList.Where(items => items.Visible == true && items.IsUnknown == false &&
        items.IsEnabled == true).ToRegularDeckDict();
    public DeckRegularDict<TR> KnownList => CardList.Where(items => items.IsUnknown == false).ToRegularDeckDict();
    public int CardSelected => CardList.Where(items => items.IsSelected == true).Select(items => items.Deck).SingleOrDefault();
}