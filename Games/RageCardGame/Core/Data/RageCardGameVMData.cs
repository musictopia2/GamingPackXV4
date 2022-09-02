namespace RageCardGame.Core.Data;
[SingletonGame]
[UseLabelGrid]
[AutoReset]
public partial class RageCardGameVMData : ITrickCardGamesData<RageCardGameCardInformation, EnumColor>
{
    private readonly RageCardGameGameContainer _gameContainer;
    private readonly IToast _toast;
    [LabelColumn]
    public string NormalTurn { get; set; } = "";
    [LabelColumn]
    public string Status { get; set; } = "";
    [LabelColumn]
    public EnumColor TrumpSuit { get; set; }
    [LabelColumn]
    public int BidAmount { get; set; } = -1;
    [LabelColumn]
    public string Lead { get; set; } = "";
    [LabelColumn]
    public EnumColor ColorChosen { get; set; }
    public RageCardGameVMData(CommandContainer command,
            SpecificTrickAreaObservable trickArea1,
            IGamePackageResolver resolver,
            RageCardGameGameContainer gameContainer,
            IToast toast
            )
    {
        Deck1 = new(command);
        Pile1 = new(command);
        PlayerHand1 = new(command);
        TrickArea1 = trickArea1;
        _gameContainer = gameContainer;
        _toast = toast;
        Color1 = new(command, new ColorListChooser<EnumColor>());
        Color1.AutoSelectCategory = EnumAutoSelectCategory.AutoEvent;
        Color1.ItemClickedAsync += Color1_ItemClickedAsync;
        Bid1 = new(command, resolver);
        Bid1.ChangedNumberValueAsync += Bid1_ChangedNumberValueAsync;
    }
    private async Task Color1_ItemClickedAsync(EnumColor piece)
    {
        if (piece == _gameContainer!.SaveRoot!.TrumpSuit && _gameContainer!.SaveRoot!.TrickList.Last().SpecialType == EnumSpecialType.Change)
        {
            _toast.ShowUserErrorToast($"{piece} is already current trump.  Choose a different one");
            return;
        }
        ColorChosen = piece;
        if (_gameContainer.ColorChosenAsync == null)
        {
            throw new CustomBasicException("Nobody is handling the color chosen.  Rethink");
        }
        await _gameContainer!.ColorChosenAsync!.Invoke();
    }
    private Task Bid1_ChangedNumberValueAsync(int chosen)
    {
        BidAmount = chosen;
        return Task.CompletedTask;
    }
    BasicTrickAreaObservable<EnumColor, RageCardGameCardInformation> ITrickCardGamesData<RageCardGameCardInformation, EnumColor>.TrickArea1
    {
        get => TrickArea1;
        set => TrickArea1 = (SpecificTrickAreaObservable)value;
    }
    public SimpleEnumPickerVM<EnumColor> Color1;
    public NumberPicker Bid1;
    public SpecificTrickAreaObservable TrickArea1 { get; set; }
    public DeckObservablePile<RageCardGameCardInformation> Deck1 { get; set; }
    public SingleObservablePile<RageCardGameCardInformation> Pile1 { get; set; }
    public HandObservable<RageCardGameCardInformation> PlayerHand1 { get; set; }
    public SingleObservablePile<RageCardGameCardInformation>? OtherPile { get; set; }
}