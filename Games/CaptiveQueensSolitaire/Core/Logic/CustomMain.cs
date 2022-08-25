namespace CaptiveQueensSolitaire.Core.Logic;
public class CustomMain : BasicMultiplePilesCP<SolitaireCard>, IMain, ISerializable //good news is if nothing is found, just will do nothing.
{
    public int CardsNeededToBegin { get; set; }
    public bool IsRound { get; set; }
#pragma warning disable 0067
    public event MainPileClickedEventHandler? PileSelectedAsync;
#pragma warning restore 0067
    private readonly IScoreData _score;

    public CustomMain(IScoreData score, CommandContainer command) : base(command)
    {
        _score = score;
    }
    public void SetSavedScore(int previousScore)
    {
        _score.Score = previousScore;
    }
    public override void ClearBoard()
    {
        base.ClearBoard();
        _score.Score = 4;
    }
    public void AddCard(int pile, SolitaireCard thisCard)
    {
        AddCardToPile(pile, thisCard);
        _score.Score++;
    }
    public void FirstLoad(bool needToMatch, bool showNextNeeded)
    {
        Columns = 8;
        Rows = 1;
        HasFrame = true;
        HasText = false;
        Style = EnumMultiplePilesStyleList.HasList;
        LoadBoard();
        static bool GetAngle(int index)
        {
            if (index == 2 || index == 3 || index == 6 || index == 7)
            {
                return true;
            }
            return false;
        }
        PileList!.ForEach(thisPile =>
        {
            int index = PileList.IndexOf(thisPile);
            thisPile.Rotated = GetAngle(index);
        });
    }
    public int HowManyPiles()
    {
        if (PileList!.Count != 8)
        {
            throw new CustomBasicException("There should have been 8 piles");
        }
        return PileList.Count;
    }
    protected override async Task OnPileClickedAsync(int Index, BasicPileInfo<SolitaireCard> ThisPile)
    {
        await PileSelectedAsync!(Index);
    }
    public bool CanAddCard(int pile, SolitaireCard thiscard)
    {
        if (HasCard(pile) == false)
        {
            return pile switch
            {
                0 => thiscard.Value == EnumRegularCardValueList.Six && thiscard.Suit == EnumSuitList.Hearts,
                1 => thiscard.Value == EnumRegularCardValueList.Five && thiscard.Suit == EnumSuitList.Hearts,
                2 => thiscard.Value == EnumRegularCardValueList.Six && thiscard.Suit == EnumSuitList.Clubs,
                3 => thiscard.Value == EnumRegularCardValueList.Five && thiscard.Suit == EnumSuitList.Clubs,
                4 => thiscard.Value == EnumRegularCardValueList.Six && thiscard.Suit == EnumSuitList.Diamonds,
                5 => thiscard.Value == EnumRegularCardValueList.Five && thiscard.Suit == EnumSuitList.Diamonds,
                6 => thiscard.Value == EnumRegularCardValueList.Six && thiscard.Suit == EnumSuitList.Spades,
                7 => thiscard.Value == EnumRegularCardValueList.Five && thiscard.Suit == EnumSuitList.Spades,
                _ => throw new CustomBasicException("Only has 8 piles"),
            };
        }
        var firstCard = PileList![pile].ObjectList.First();
        var lastCard = GetLastCard(pile);
        if (lastCard.Suit != firstCard.Suit)
        {
            throw new CustomBasicException("Must match suit");
        }
        if (lastCard.Suit != thiscard.Suit)
        {
            return false;
        }
        if (lastCard.Value.Value + 1 == thiscard.Value.Value && firstCard.Value == EnumRegularCardValueList.Six)
        {
            return true;
        }
        if (lastCard.Value.Value - 1 == thiscard.Value.Value && firstCard.Value == EnumRegularCardValueList.Five)
        {
            return true;
        }
        return firstCard.Value == EnumRegularCardValueList.Five && lastCard.Value == EnumRegularCardValueList.LowAce && thiscard.Value == EnumRegularCardValueList.King;
    }
    public int StartNumber()
    {
        throw new NotImplementedException(); // until i decide what to do
    }
    public void ClearBoard(IDeckDict<SolitaireCard> thisList) //somehow was never done.
    {
    }
    public async Task LoadGameAsync(string data)
    {
        PileList = await js.DeserializeObjectAsync<BasicList<BasicPileInfo<SolitaireCard>>>(data);
    }
    public async Task<string> GetSavedPilesAsync()
    {
        return await js.SerializeObjectAsync(PileList!);
    }
    public void AddCards(int Pile, IDeckDict<SolitaireCard> list)
    {

    }
    public void AddCards(IDeckDict<SolitaireCard> list)
    {

    }
}