namespace FillOrBust.Core.Data;
[SingletonGame]
[UseLabelGrid]
[AutoReset]
public partial class FillOrBustVMData(CommandContainer command, IGamePackageResolver resolver) : IBasicCardGamesData<FillOrBustCardInformation>, ICup<SimpleDice>
{
    [LabelColumn]
    public partial string NormalTurn { get; set; } = "";
    [LabelColumn]
    public partial string Status { get; set; } = "";
    [LabelColumn]
    public partial int DiceScore { get; set; }
    [LabelColumn]
    public partial int TempScore { get; set; }
    [LabelColumn]
    public partial string Instructions { get; set; } = "";
    public void LoadCup(FillOrBustSaveInfo saveRoot, bool autoResume)
    {
        if (Cup != null && autoResume)
        {
            return;
        }
        Cup = new DiceCup<SimpleDice>(saveRoot.DiceList, resolver, command);
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
    public DeckObservablePile<FillOrBustCardInformation> Deck1 { get; set; } = new(command);
    public SingleObservablePile<FillOrBustCardInformation> Pile1 { get; set; } = new(command);
    public HandObservable<FillOrBustCardInformation> PlayerHand1 { get; set; } = new(command);
    public SingleObservablePile<FillOrBustCardInformation>? OtherPile { get; set; }
}