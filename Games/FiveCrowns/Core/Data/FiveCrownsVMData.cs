namespace FiveCrowns.Core.Data;
[SingletonGame]
[UseLabelGrid]
[AutoReset]
public partial class FiveCrownsVMData : IBasicCardGamesData<FiveCrownsCardInformation>
{
    [LabelColumn]
    public string NormalTurn { get; set; } = "";
    [LabelColumn]
    public string Status { get; set; } = "";
    [LabelColumn]
    public int UpTo { get; set; }
    public FiveCrownsVMData(CommandContainer command, IGamePackageResolver resolver)
    {
        Deck1 = new(command);
        Pile1 = new(command);
        PlayerHand1 = new(command)
        {
            Maximum = 14
        };
        TempSets = new(command, resolver);
        MainSets = new(command);
        TempSets.HowManySets = 6;
    }
    public TempSetsObservable<EnumSuitList, EnumColorList, FiveCrownsCardInformation> TempSets;
    public MainSetsObservable<EnumSuitList, EnumColorList, FiveCrownsCardInformation, PhaseSet, SavedSet> MainSets;
    public DeckObservablePile<FiveCrownsCardInformation> Deck1 { get; set; }
    public SingleObservablePile<FiveCrownsCardInformation> Pile1 { get; set; }
    public HandObservable<FiveCrownsCardInformation> PlayerHand1 { get; set; }
    public SingleObservablePile<FiveCrownsCardInformation>? OtherPile { get; set; }
}