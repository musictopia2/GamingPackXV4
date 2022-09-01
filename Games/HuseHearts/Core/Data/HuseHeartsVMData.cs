namespace HuseHearts.Core.Data;
[SingletonGame]
[UseLabelGrid]
[AutoReset]
public partial class HuseHeartsVMData : ITrickCardGamesData<HuseHeartsCardInformation, EnumSuitList>,
    ITrickDummyHand<EnumSuitList, HuseHeartsCardInformation>
{
    private readonly HuseHeartsGameContainer _gameContainer;
    [LabelColumn]
    public string NormalTurn { get; set; } = "";
    [LabelColumn]
    public string Status { get; set; } = "";
    [LabelColumn]
    public EnumSuitList TrumpSuit { get; set; }
    public HuseHeartsVMData(CommandContainer command,
           HuseHeartsTrickAreaCP trickArea1,
           HuseHeartsGameContainer gameContainer
           )
    {
        Deck1 = new(command);
        Pile1 = new(command);
        PlayerHand1 = new(command);
        TrickArea1 = trickArea1;
        _gameContainer = gameContainer;
        Dummy1 = new(command);
        Blind1 = new(command);
        Blind1.Maximum = 4;
        Blind1.Text = "Blind";
        Dummy1.Text = "Dummy Hand";
        Dummy1.AutoSelect = EnumHandAutoType.SelectOneOnly;
    }
    public Action? ChangeScreen { get; set; }
    public HandObservable<HuseHeartsCardInformation> Dummy1;
    public HandObservable<HuseHeartsCardInformation> Blind1;
    BasicTrickAreaObservable<EnumSuitList, HuseHeartsCardInformation> ITrickCardGamesData<HuseHeartsCardInformation, EnumSuitList>.TrickArea1
    {
        get => TrickArea1;
        set => TrickArea1 = (HuseHeartsTrickAreaCP)value;
    }
    public HuseHeartsTrickAreaCP TrickArea1 { get; set; }
    public DeckObservablePile<HuseHeartsCardInformation> Deck1 { get; set; }
    public SingleObservablePile<HuseHeartsCardInformation> Pile1 { get; set; }
    public HandObservable<HuseHeartsCardInformation> PlayerHand1 { get; set; }
    public SingleObservablePile<HuseHeartsCardInformation>? OtherPile { get; set; }
    [LabelColumn]
    public int RoundNumber { get; set; }
    private EnumStatus _gameStatus;
    [LabelColumn]
    public EnumStatus GameStatus
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
    public DeckRegularDict<HuseHeartsCardInformation> GetCurrentHandList()
    {
        if (TrickArea1!.FromDummy == true)
        {
            return Dummy1!.HandList;
        }
        else
        {
            return _gameContainer!.SingleInfo!.MainHandList;
        }
    }
    public int CardSelected()
    {
        if (TrickArea1!.FromDummy == true)
        {
            return Dummy1!.ObjectSelected();
        }
        else if (_gameContainer!.SingleInfo!.PlayerCategory != EnumPlayerCategory.Self)
        {
            throw new CustomBasicException("Only self can show card selected.  If I am wrong, rethink");
        }
        return PlayerHand1!.ObjectSelected();
    }
    public void RemoveCard(int deck)
    {
        if (TrickArea1!.FromDummy == true)
        {
            Dummy1!.HandList.RemoveObjectByDeck(deck);
        }
        else
        {
            _gameContainer.SingleInfo!.MainHandList.RemoveObjectByDeck(deck);
        }
    }
}