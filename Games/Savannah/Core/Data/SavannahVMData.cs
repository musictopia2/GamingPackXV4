namespace Savannah.Core.Data;
[SingletonGame]
[UseLabelGrid]
[AutoReset]
public partial class SavannahVMData : IBasicCardGamesData<RegularSimpleCard>, ICup<SimpleDice>
{
    private readonly CommandContainer _command;
    private readonly IGamePackageResolver _resolver;
    [LabelColumn]
    public string NormalTurn { get; set; } = "";
    [LabelColumn]
    public string Status { get; set; } = "";
    public SavannahVMData(CommandContainer command, IGamePackageResolver resolver)
    {
        Deck1 = new DeckObservablePile<RegularSimpleCard>(command);
        Pile1 = new SingleObservablePile<RegularSimpleCard>(command);
        Pile1.Visible = false;
        PlayerHand1 = new HandObservable<RegularSimpleCard>(command);
        PublicPiles = new(command);
        SelfStock = new(command);
        _command = command;
        _resolver = resolver;
    }
    public void LoadCup(SavannahSaveInfo saveRoot, bool autoResume)
    {
        if (Cup != null && autoResume)
        {
            return;
        }
        Cup = new(saveRoot.DiceList, _resolver, _command)
        {
            CanShowDice = true,
            HowManyDice = 2 //there are 2 dice for this game.
        };
    }
    public SelfDiscardCP? SelfDiscard { get; set; }
    public PublicPilesViewModel PublicPiles;
    public StockViewModel SelfStock { get; set; }
    public DiceCup<SimpleDice>? Cup { get; set; }
    public DeckObservablePile<RegularSimpleCard> Deck1 { get; set; }
    public SingleObservablePile<RegularSimpleCard> Pile1 { get; set; }
    public HandObservable<RegularSimpleCard> PlayerHand1 { get; set; }
    public SingleObservablePile<RegularSimpleCard>? OtherPile { get; set; }
    //any other ui related properties will be here.
    //can copy/paste for the actual view model.
}