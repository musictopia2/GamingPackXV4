namespace FillOrBust.Core.Data;
[SingletonGame]
[UseLabelGrid]
[AutoReset]
public partial class FillOrBustVMData : IBasicCardGamesData<FillOrBustCardInformation>, ICup<SimpleDice>
{
    [LabelColumn]
    public string NormalTurn { get; set; } = "";
    [LabelColumn]
    public string Status { get; set; } = "";
    [LabelColumn]
    public int DiceScore { get; set; }
    [LabelColumn]
    public int TempScore { get; set; }
    [LabelColumn]
    public string Instructions { get; set; } = "";
    private readonly CommandContainer _command;
    private readonly IGamePackageResolver _resolver;
    public FillOrBustVMData(CommandContainer command, IGamePackageResolver resolver)
    {
        Deck1 = new(command);
        Pile1 = new(command);
        PlayerHand1 = new(command);
        _command = command;
        _resolver = resolver;
    }
    public void LoadCup(FillOrBustSaveInfo saveRoot, bool autoResume)
    {
        if (Cup != null && autoResume)
        {
            return;
        }
        Cup = new DiceCup<SimpleDice>(saveRoot.DiceList, _resolver, _command);
        if (autoResume == true)
        {
            Cup.CanShowDice = true;
        }
        else
        {
            Cup.HowManyDice = 6;
        }
    }

    public DiceCup<SimpleDice>? Cup { get; set; }
    public DeckObservablePile<FillOrBustCardInformation> Deck1 { get; set; }
    public SingleObservablePile<FillOrBustCardInformation> Pile1 { get; set; }
    public HandObservable<FillOrBustCardInformation> PlayerHand1 { get; set; }
    public SingleObservablePile<FillOrBustCardInformation>? OtherPile { get; set; }
}