namespace BasicGameFrameworkLibrary.Core.DrawableListsObservable;
public class SavedScatteringPieces<D> where D : IDeckObject, new()
{
    public bool HasDrawn { get; set; }
    public bool ClearedOut { get; set; }
    public DeckRegularDict<D> RemainingList { get; set; } = new();
}
public abstract partial class ScatteringPiecesObservable<D, L> : SimpleControlObservable where D : ILocationDeck, new()
    where L : class, IScatterList<D>, new()
{
    protected abstract Task ClickedPieceAsync(int Deck); // deck is the card number (needs to be deck so each one can use the function to return the proper card
    protected abstract Task ClickedBoardAsync();
    private bool _mVarHasDrawn;
    private bool _clearedOut;
    protected L? ObjectList;
    protected string ProtectedText = ""; // text would already be given when doing this
    public DeckRegularDict<D> RemainingList = new();
    public ControlCommand? ObjectCommand { get; set; } // needs the commands
    public ControlCommand? BoardCommand { get; set; }
    private bool _privateObjectClicked; //no need for tags because no proportions for blazor
    private readonly IRandomGenerator _rs;
    private readonly IEventAggregator _thisE;
    [Command(EnumCommandCategory.Control, Name = nameof(ObjectCommand))]
    private async Task PrivateClickItemAsync(D card)
    {
        _privateObjectClicked = true;
        await ClickedPieceAsync(card.Deck);
    }
    [Command(EnumCommandCategory.Control, Name = nameof(BoardCommand))]
    private async Task PrivateBoardClickAsync()
    {
        if (_privateObjectClicked == true)
        {
            _privateObjectClicked = false;
            return;
        }
        await ClickedBoardAsync();
    }
    public ScatteringPiecesObservable(CommandContainer command, IGamePackageResolver resolver) : base(command)
    {
        _rs = resolver.Resolve<IRandomGenerator>();
        _thisE = resolver.Resolve<IEventAggregator>();
        CreateCommands();
    }
    partial void CreateCommands();
    public SizeF MaxSize { get; set; }
    public string TextToAppear { get; set; } = "";
    public bool CanDraw { get; set; } = true;
    public void LoadPlayerPieces(IDeckDict<D> thisList, ref DeckRegularDict<D> hand, bool isFirst)
    {
        hand.ReplaceRange(thisList);
        if (isFirst == false)
        {
            return;
        }
        DeckRegularDict<D> newList = new();
        foreach (var thisItem in thisList)
        {
            newList.Add(RemainingList.GetSpecificItem(thisItem.Deck));
        }
        RemovePieces(newList);
    }
    public void SavedGame(SavedScatteringPieces<D> save)
    {
        _mVarHasDrawn = save.HasDrawn;
        _clearedOut = save.ClearedOut;
        RemainingList.ReplaceRange(save.RemainingList);
    }
    public SavedScatteringPieces<D> SavedData()
    {
        SavedScatteringPieces<D> output = new();
        output.HasDrawn = _mVarHasDrawn;
        output.ClearedOut = _clearedOut;
        output.RemainingList = RemainingList.ToRegularDeckDict();
        return output;
    }
    protected int DrawPiece()
    {
        return RemainingList.GetRandomItem().Deck;
    }
    protected void RemoveSinglePiece(int deck)
    {
        var thisPiece = RemainingList.GetSpecificItem(deck);
        RemainingList.RemoveSpecificItem(thisPiece);
        _mVarHasDrawn = true;
    }
    protected void RemovePieces(IDeckDict<D> thisList)
    {
        RemainingList.RemoveGivenList(thisList);
    }
    protected void GetFirstPieces(int howMany, out DeckRegularDict<D> thisList)
    {
        if (ObjectList!.Count == 0)
        {
            throw new Exception("There must be at least one piece in order to get the first pieces");
        }
        thisList = new DeckRegularDict<D>(); //i think.
        if (RemainingList.Count < howMany)
        {
            throw new Exception("There are only " + RemainingList.Count + " but was trying to get " + howMany + " pieces");
        }
        IBasicList<D> newList;
        newList = RemainingList.GetRandomList(true, howMany);
        if (RemainingList.Count == 0)
        {
            throw new Exception("Can't have 0 pieces remaining after getting random list");
        }
        foreach (var thisCard in newList)
            thisCard.IsUnknown = false;
        thisList.ReplaceRange(newList);
    }
    public void ScatterPieces()
    {
        int maxPointx;
        int maxPointy;
        if (MaxSize.Width == 0 || MaxSize.Height == 0)
        {
            throw new Exception("Must specify height and width in order to scatter the pieces");
        }
        if (RemainingList.Count == 0)
        {
            return;// nothing to scatter anymore.
        }
        D tempObject;
        tempObject = RemainingList.First();
        SizeF size = tempObject.DefaultSize;
        maxPointx = (int)MaxSize.Width - (int)size.Width - 3;
        maxPointy = (int)MaxSize.Height - (int)size.Height - 3;
        foreach (var thisCard in RemainingList)
        {
            var locx = _rs.GetRandomNumber(maxPointx, 3);
            var locy = _rs.GetRandomNumber(maxPointy, 20);
            thisCard.Location = new PointF(locx, locy); // you need location now.
        }
    }
    public void PopulateBoard() //try this way this time.
    {
        ObjectList!.ClearObjects(); //i think.
        ObjectList.ShuffleObjects();
        if (ObjectList.Count == 0)
        {
            throw new Exception("Must have at least one piece after reshuffling");
        }
        foreach (var thisCard in ObjectList)
        {
            thisCard.IsUnknown = true;
        }
        RemainingList.ReplaceRange(ObjectList);
        if (MaxSize.Width > 0 && MaxSize.Height > 0)
        {
            ScatterPieces(); // the next time, the cross platform processes has to do it.
            _thisE.Publish(new ScatteringCompletedEventModel()); //could be iffy (?)
        }
    }
    public void PopulateTotals()
    {
        if (ProtectedText == "")
        {
            throw new Exception("The text must be filled out.");
        }
        int totals;
        if (_clearedOut == true)
        {
            totals = 0;
        }
        else
        {
            totals = RemainingList.Count;// try this way.
        }
        TextToAppear = ProtectedText + " (" + totals + ")";
    }
    protected void EmptyBoard()
    {
        _clearedOut = true;
        PopulateTotals();
    }
    public void NewTurn()
    {
        _mVarHasDrawn = false;
    }
    public bool HasDrawn()
    {
        return _mVarHasDrawn;
    }
    protected override bool CanEnableFirst()
    {
        if (CanDraw == false)
        {
            return false;
        }
        if (_mVarHasDrawn == true)
        {
            return false;
        }
        if (HasPieces() == false)
        {
            return false;
        }
        return true;
    }
    protected bool HasPieces()
    {
        if (RemainingList.Count == 0)
        {
            return false;
        }
        if (_clearedOut == true)
        {
            return false;
        }
        return true;
    }
    protected override void EnableChange() { }
}