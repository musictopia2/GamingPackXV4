namespace MonasteryCardGame.Core.Data;
[SingletonGame]
[UseLabelGrid]
[AutoReset]
public partial class MonasteryCardGameVMData : IBasicCardGamesData<MonasteryCardInfo>
{
    [LabelColumn]
    public string NormalTurn { get; set; } = "";
    [LabelColumn]
    public string Status { get; set; } = "";
    [LabelColumn]
    public string MissionChosen { get; set; } = "";
    public MonasteryCardGameVMData(CommandContainer command, IGamePackageResolver resolver)
    {
        Deck1 = new DeckObservablePile<MonasteryCardInfo>(command);
        Pile1 = new SingleObservablePile<MonasteryCardInfo>(command);
        PlayerHand1 = new(command);
        PlayerHand1.Maximum = 10;
        TempSets = new(command, resolver);
        MainSets = new(command);
        TempSets.HowManySets = 4;
    }
    public DeckObservablePile<MonasteryCardInfo> Deck1 { get; set; }
    public SingleObservablePile<MonasteryCardInfo> Pile1 { get; set; }
    public HandObservable<MonasteryCardInfo> PlayerHand1 { get; set; }
    public SingleObservablePile<MonasteryCardInfo>? OtherPile { get; set; }
    public TempSetsObservable<EnumSuitList, EnumRegularColorList, MonasteryCardInfo> TempSets;
    public MainSetsObservable<EnumSuitList, EnumRegularColorList, MonasteryCardInfo, RummySet, SavedSet> MainSets;
    public BasicList<MissionList> CompleteMissions = new();
    internal void PopulateMissions(BasicList<MissionList> thisList)
    {
        MissionChosen = "";
        CompleteMissions.ReplaceRange(thisList);
    }
}