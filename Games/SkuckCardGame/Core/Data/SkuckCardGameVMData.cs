namespace SkuckCardGame.Core.Data;
[SingletonGame]
[UseLabelGrid]
[AutoReset]
public partial class SkuckCardGameVMData : ITrickCardGamesData<SkuckCardGameCardInformation, EnumSuitList>
    , ITrickDummyHand<EnumSuitList, SkuckCardGameCardInformation>
{
    [LabelColumn]
    public string NormalTurn { get; set; } = "";
    [LabelColumn]
    public string Status { get; set; } = "";
    [LabelColumn]
    public EnumSuitList TrumpSuit { get; set; }
    private readonly SkuckCardGameGameContainer _gameContainer;
    public SkuckCardGameVMData(CommandContainer command,
            BasicTrickAreaObservable<EnumSuitList, SkuckCardGameCardInformation> trickArea1,
            IGamePackageResolver resolver,
            SkuckCardGameGameContainer gameContainer
            )
    {
        Deck1 = new(command);
        Pile1 = new(command);
        PlayerHand1 = new(command);
        TrickArea1 = trickArea1;
        _gameContainer = gameContainer;
        Bid1 = new(command, resolver);
        Suit1 = new(command, new SuitListChooser());
        Suit1.AutoSelectCategory = EnumAutoSelectCategory.AutoSelect;
        Bid1.ChangedNumberValueAsync = Bid1_ChangedNumberValueAsync;
        Suit1.ItemSelectionChanged = Suit1_ItemSelectionChanged;
        Bid1.LoadNormalNumberRangeValues(1, 26);
    }
    public Action? ChangeScreen { get; set; }
    public NumberPicker Bid1;
    public SimpleEnumPickerVM<EnumSuitList> Suit1;
    public BasicTrickAreaObservable<EnumSuitList, SkuckCardGameCardInformation> TrickArea1 { get; set; }
    public DeckObservablePile<SkuckCardGameCardInformation> Deck1 { get; set; }
    public SingleObservablePile<SkuckCardGameCardInformation> Pile1 { get; set; }
    public HandObservable<SkuckCardGameCardInformation> PlayerHand1 { get; set; }
    public SingleObservablePile<SkuckCardGameCardInformation>? OtherPile { get; set; }
    [LabelColumn]
    public int RoundNumber { get; set; }
    private EnumStatusList _gameStatus;
    public EnumStatusList GameStatus
    {
        get { return _gameStatus; }
        set
        {
            if (SetProperty(ref _gameStatus, value))
            {
                ChangeScreen?.Invoke();
            }
        }
    }
    [LabelColumn]
    public int BidAmount { get; set; } = -1;
    DeckRegularDict<SkuckCardGameCardInformation> ITrickDummyHand<EnumSuitList, SkuckCardGameCardInformation>.GetCurrentHandList()
    {
        DeckRegularDict<SkuckCardGameCardInformation> output = _gameContainer!.SingleInfo!.MainHandList.ToRegularDeckDict();
        output.AddRange(_gameContainer.SingleInfo.TempHand!.ValidCardList);
        return output;
    }
    int ITrickDummyHand<EnumSuitList, SkuckCardGameCardInformation>.CardSelected()
    {
        if (_gameContainer!.SingleInfo!.PlayerCategory != EnumPlayerCategory.Self)
        {
            throw new CustomBasicException("Only self can get card selected.  If I am wrong, rethink");
        }
        int selects = PlayerHand1!.ObjectSelected();
        int others = _gameContainer.SingleInfo.TempHand!.CardSelected;
        if (selects != 0 && others != 0)
        {
            throw new CustomBasicException("You cannot choose from both hand and temps.  Rethink");
        }
        if (selects != 0)
        {
            return selects;
        }
        return others;
    }
    void ITrickDummyHand<EnumSuitList, SkuckCardGameCardInformation>.RemoveCard(int deck)
    {
        bool rets = _gameContainer!.SingleInfo!.MainHandList.ObjectExist(deck);
        if (rets == true)
        {
            _gameContainer.SingleInfo.MainHandList.RemoveObjectByDeck(deck);
            return;
        }
        var thisCard = _gameContainer.SingleInfo.TempHand!.CardList.GetSpecificItem(deck);
        if (thisCard.IsEnabled == false)
        {
            throw new CustomBasicException("Card was supposed to be disabled");
        }
        _gameContainer.SingleInfo.TempHand.HideCard(thisCard);
    }
    private Task Bid1_ChangedNumberValueAsync(int chosen)
    {
        BidAmount = chosen;
        return Task.CompletedTask;
    }
    private void Suit1_ItemSelectionChanged(EnumSuitList piece)
    {
        _gameContainer.SaveRoot.TrumpSuit = piece;
    }
}