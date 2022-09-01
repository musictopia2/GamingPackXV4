namespace HorseshoeCardGame.Core.Data;
[SingletonGame]
[UseLabelGrid]
[AutoReset]
public partial class HorseshoeCardGameVMData : ITrickCardGamesData<HorseshoeCardGameCardInformation, EnumSuitList>,
    ITrickDummyHand<EnumSuitList, HorseshoeCardGameCardInformation>
{
    private readonly HorseshoeCardGameGameContainer _gameContainer;
    [LabelColumn]
    public string NormalTurn { get; set; } = "";
    [LabelColumn]
    public string Status { get; set; } = "";
    [LabelColumn]
    public EnumSuitList TrumpSuit { get; set; }
    public HorseshoeCardGameVMData(CommandContainer command,
            HorseshoeTrickAreaCP trickArea1,
            HorseshoeCardGameGameContainer gameContainer
            )
    {
        Deck1 = new(command);
        Pile1 = new(command);
        PlayerHand1 = new(command);
        TrickArea1 = trickArea1;
        TrickArea1 = trickArea1;
        _gameContainer = gameContainer;
        PlayerHand1.Maximum = 6;
        gameContainer.GetCurrentHandList = GetCurrentHandList;
    }
    public HorseshoeTrickAreaCP TrickArea1 { get; set; }
    BasicTrickAreaObservable<EnumSuitList, HorseshoeCardGameCardInformation> ITrickCardGamesData<HorseshoeCardGameCardInformation, EnumSuitList>.TrickArea1
    {
        get => TrickArea1;
        set => TrickArea1 = (HorseshoeTrickAreaCP)value;
    }
    public DeckObservablePile<HorseshoeCardGameCardInformation> Deck1 { get; set; }
    public SingleObservablePile<HorseshoeCardGameCardInformation> Pile1 { get; set; }
    public HandObservable<HorseshoeCardGameCardInformation> PlayerHand1 { get; set; }
    public SingleObservablePile<HorseshoeCardGameCardInformation>? OtherPile { get; set; }
    public DeckRegularDict<HorseshoeCardGameCardInformation> GetCurrentHandList()
    {
        DeckRegularDict<HorseshoeCardGameCardInformation> output = _gameContainer!.SingleInfo!.MainHandList.ToRegularDeckDict();
        output.AddRange(_gameContainer.SingleInfo.TempHand!.ValidCardList);
        return output;
    }
    public int CardSelected()
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
    public void RemoveCard(int deck)
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
}