namespace BasicGameFrameworkLibrary.Core.SolitaireClasses.PileObservable;
public class MainPilesCP : IMain, ISerializable
{
    public int CardsNeededToBegin { get; set; }
    public bool IsRound { get; set; }
    public int Rows { get; set; }
    public int Columns { get; set; }
    private bool _suitsNeedToMatch;
    internal bool ShowNextNeeded { get; set; }
    protected int Increments = 1;
    public Func<int, Task>? PileSelectedAsync { get; set; }
    public BasicMultiplePilesCP<SolitaireCard> Piles;
    protected IRegularDeckInfo DeckContents;
    private readonly IScoreData _score;
    public MainPilesCP(IScoreData thisMod, CommandContainer command, IGamePackageResolver resolver)
    {
        _score = thisMod;
        DeckContents = resolver.Resolve<IRegularDeckInfo>();
        Piles = new BasicMultiplePilesCP<SolitaireCard>(command);
        Piles.PileClickedAsync = Piles_PileClickedAsync;
    }
    private async Task Piles_PileClickedAsync(int index, BasicPileInfo<SolitaireCard> thisPile)
    {
        if (PileSelectedAsync is null)
        {
            return;
        }
        await PileSelectedAsync.Invoke(index);
    }
    public void SetSavedScore(int score)
    {
        _score.Score = score;
    }
    public virtual void ClearBoard(IDeckDict<SolitaireCard> thisList)
    {
        if (thisList.Count != CardsNeededToBegin)
        {
            throw new CustomBasicException($"Needs {CardsNeededToBegin} not {thisList.Count}");
        }
        _score.Score = thisList.Count;
        if (CardsNeededToBegin != Rows * Columns && CardsNeededToBegin != 0 && IsRound == false)
        {
            throw new CustomBasicException($"Since there are no 0 cards needed; then needs {Rows * Columns}, not {CardsNeededToBegin}");
        }
        else if (IsRound && CardsNeededToBegin != 1)
        {
            throw new CustomBasicException("When its round suits; needs only one card to begin with");
        }
        else if (IsRound && thisList.Count != 1)
        {
            throw new CustomBasicException("When its round; needs to have exactly one card to begin with");
        }
        int y = 2;
        Piles.PileList!.ForEach(thisPile =>
        {
            if (ShowNextNeeded)
            {
                thisPile.Text = y.ToString();
            }
            y += 2;
        });
        Piles.ClearBoard();
        if (thisList.Count == 0)
        {
            return;
        }
        int x = 0;
        foreach (var thisPile in Piles.PileList)
        {
            thisPile.ObjectList.Add(thisList[x]);
            x++;
            if (IsRound)
            {
                return;
            }
        }
    }
    public void ClearBoard()
    {
        if (CardsNeededToBegin != 0)
        {
            throw new CustomBasicException("Must send a collection since this game requires cards to begin with");
        }
        DeckRegularDict<SolitaireCard> tempList = new();
        ClearBoard(tempList);
    }
    protected static int FindNextNeeded(int pile, int oldNumber)
    {
        if (oldNumber == 13)
        {
            return 0;
        }
        int diffs = 0;
        do
        {
            oldNumber++;
            if (oldNumber > 13)
            {
                oldNumber = 1;
            }
            if (diffs == pile)
            {
                return oldNumber;
            }
            diffs++;
        } while (true);
    }
    public void AddCard(int pile, SolitaireCard thisCard)
    {
        if (thisCard.Suit == EnumSuitList.None)
        {
            throw new CustomBasicException("The suit cannot be none.  Hint:  May be a problem with cloning.  Find out what happened");
        }
        if (ShowNextNeeded)
        {
            var thisPile = Piles.PileList![pile];
            thisPile.Text = FindNextNeeded(pile, thisCard.Value.Value).ToString();
            if (thisPile.Text == "0")
            {
                thisPile.IsEnabled = false;
            }
        }
        Piles.AddCardToPile(pile, thisCard);
        _score.Score++;
    }
    public void FirstLoad(bool needToMatch, bool _showNextNeeded)
    {
        ShowNextNeeded = _showNextNeeded;
        Piles.Rows = Rows;
        Piles.Columns = Columns;
        Piles.HasFrame = true;
        Piles.HasText = ShowNextNeeded;
        Piles.Style = EnumMultiplePilesStyleList.HasList;
        _suitsNeedToMatch = needToMatch;
        Piles.LoadBoard();
    }
    public void AddCards(int pile, IDeckDict<SolitaireCard> list)
    {
        list.ForEach(thisCard =>
        {
            Piles.AddCardToPile(pile, thisCard);
        });
        _score.Score += list.Count;
    }
    public void AddCards(IDeckDict<SolitaireCard> list)
    {
        for (int x = 0; x < list.Count; x++)
        {
            if (Piles.HasCard(x) == false)
            {
                AddCards(x, list);
                return;
            }
        }
        throw new CustomBasicException("There was no empty columns");
    }
    public int HowManyPiles()
    {
        return Piles.PileList!.Count;
    }
    public bool HasCard(int pile)
    {
        return Piles.HasCard(pile);
    }
    public int StartNumber()
    {
        if (IsRound == false)
        {
            return 0;
        }
        return Piles.PileList!.First().ObjectList.First().Value.Value;
    }
    public virtual bool CanAddCard(int pile, SolitaireCard thisCard)
    {
        if (Piles.HasCard(pile) == false)
        {
            if (thisCard.Value.Value == Increments && IsRound == false)
            {
                return true;
            }
            int starts = StartNumber();
            return thisCard.Value.Value == starts && IsRound;
        }
        var thisPile = Piles.PileList![pile];
        if (ShowNextNeeded)
        {
            return int.Parse(thisPile.Text) == thisCard.Value.Value;
        }
        var oldCard = Piles.GetLastCard(pile);
        if (_suitsNeedToMatch)
        {
            if (oldCard.Suit != thisCard.Suit)
            {
                return false;
            }
        }
        if (thisPile.ObjectList.Count == 1 && Increments == 1 && DeckContents.LowestNumber > 2 && IsRound == false)
        {
            return thisCard.Value.Value == DeckContents.LowestNumber;
        }
        if (oldCard.Value.Value + Increments == thisCard.Value.Value)
        {
            return true;
        }
        if (IsRound && Increments == 1 && oldCard.Value == EnumRegularCardValueList.King && thisCard.Value == EnumRegularCardValueList.LowAce)
        {
            return true;
        }
        if (oldCard.Value.Value + Increments <= 13)
        {
            return false;
        }
        if (Increments == 1)
        {
            return false;
        }
        int diffs = 0;
        int olds = oldCard.Value.Value;
        do
        {
            olds++;
            diffs++;
            if (olds > 13)
            {
                olds = 1;
            }
            if (thisCard.Value.Value == olds)
            {
                break;
            }
            if (diffs > 13)
            {
                throw new CustomBasicException("The difference cannot be more than 13.  Find out what happened");
            }
        } while (true);
        return diffs == Increments;
    }
    public async Task LoadGameAsync(string data)
    {
        BasicList<BasicPileInfo<SolitaireCard>> tempList = await js.DeserializeObjectAsync<BasicList<BasicPileInfo<SolitaireCard>>>(data);
        Piles.PileList = tempList;
    }
    public async Task<string> GetSavedPilesAsync()
    {
        return await js.SerializeObjectAsync(Piles.PileList!);
    }
}