namespace Rook.Core.Data;
[SingletonGame]
[UseLabelGrid]
[AutoReset]
public partial class RookVMData : ITrickCardGamesData<RookCardInformation, EnumColorTypes>,
    ITrickDummyHand<EnumColorTypes, RookCardInformation>
{
    private readonly RookGameContainer _gameContainer;
    [LabelColumn]
    public string NormalTurn { get; set; } = "";
    [LabelColumn]
    public string Status { get; set; } = "";
    [LabelColumn]
    public EnumColorTypes TrumpSuit { get; set; }
    [LabelColumn]
    public EnumColorTypes ColorChosen { get; set; }
    [LabelColumn]
    public int BidChosen { get; set; } = -1;
    [LabelColumn]
    public EnumStatusList GameStatus { get; set; }
    [LabelColumn]
    public string TeamMate { get; set; } = "None"; //the players name.  only for ui.
    public RookVMData(CommandContainer command,
            RookTrickAreaCP trickArea1,
            IGamePackageResolver resolver,
            RookGameContainer gameContainer
            )
    {
        Deck1 = new(command);
        Pile1 = new(command);
        PlayerHand1 = new(command);
        TrickArea1 = trickArea1;
        _gameContainer = gameContainer;
        Bid1 = new NumberPicker(command, resolver);
        Color1 = new SimpleEnumPickerVM<EnumColorTypes>(command, new ColorListChooser<EnumColorTypes>());
        Dummy1 = new DummyHandCP(command);
        Bid1.ChangedNumberValueAsync = Bid1_ChangedNumberValueAsync;
        Color1.AutoSelectCategory = EnumAutoSelectCategory.AutoEvent;
        Color1.ItemClickedAsync = Color1_ItemClickedAsync;
    }
    //public Action? ChangeScreen { get; set; }
    private Task Bid1_ChangedNumberValueAsync(int chosen)
    {
        BidChosen = chosen;
        return Task.CompletedTask;
    }
    private Task Color1_ItemClickedAsync(EnumColorTypes piece)
    {
        ColorChosen = piece;
        TrumpSuit = piece;
        return Task.CompletedTask;
    }
    public NumberPicker Bid1;
    public SimpleEnumPickerVM<EnumColorTypes> Color1;
    public DummyHandCP Dummy1;
    public RookTrickAreaCP TrickArea1 { get; set; }
    BasicTrickAreaObservable<EnumColorTypes, RookCardInformation> ITrickCardGamesData<RookCardInformation, EnumColorTypes>.TrickArea1
    {
        get => TrickArea1;
        set => TrickArea1 = (RookTrickAreaCP)value;
    }
    public DeckObservablePile<RookCardInformation> Deck1 { get; set; }
    public SingleObservablePile<RookCardInformation> Pile1 { get; set; }
    public HandObservable<RookCardInformation> PlayerHand1 { get; set; }
    public SingleObservablePile<RookCardInformation>? OtherPile { get; set; }
    public DeckRegularDict<RookCardInformation> GetCurrentHandList()
    {
        if (_gameContainer!.SaveRoot!.DummyPlay == true && _gameContainer.PlayerList!.Count == 2)
        {
            return Dummy1!.HandList;
        }
        return _gameContainer.SingleInfo!.MainHandList;
    }
    public int CardSelected()
    {
        if (_gameContainer!.SaveRoot!.DummyPlay == true && _gameContainer.PlayerList!.Count == 2)
        {
            return Dummy1!.ObjectSelected();
        }
        else if (_gameContainer.SingleInfo!.PlayerCategory != EnumPlayerCategory.Self)
        {
            throw new CustomBasicException("Only self can show card selected.  If I am wrong, rethink");
        }
        return PlayerHand1!.ObjectSelected();
    }
    public void RemoveCard(int deck)
    {
        if (_gameContainer!.SaveRoot!.DummyPlay == true && _gameContainer.PlayerList!.Count == 2)
        {
            Dummy1!.RemoveDummyCard(deck);
        }
        else
        {
            _gameContainer.SingleInfo!.MainHandList.RemoveObjectByDeck(deck); //because computer player does this too.
        }
    }
    internal bool CanPass { get; set; }
}