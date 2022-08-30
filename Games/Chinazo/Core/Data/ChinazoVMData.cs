namespace Chinazo.Core.Data;
[SingletonGame]
[UseLabelGrid]
[AutoReset]
public partial class ChinazoVMData : IBasicCardGamesData<ChinazoCard>
{
    [LabelColumn]
    public string NormalTurn { get; set; } = "";
    [LabelColumn]
    public string Status { get; set; } = "";
    [LabelColumn]
    public string PhaseData { get; set; } = "";
    [LabelColumn]
    public string OtherLabel { get; set; } = "";
    public ChinazoVMData(CommandContainer command, IGamePackageResolver resolver)
    {
        Deck1 = new DeckObservablePile<ChinazoCard>(command);
        Pile1 = new SingleObservablePile<ChinazoCard>(command);
        PlayerHand1 = new HandObservable<ChinazoCard>(command);
        TempSets = new TempSetsObservable<EnumSuitList, EnumRegularColorList, ChinazoCard>(command, resolver);
        MainSets = new MainSetsObservable<EnumSuitList, EnumRegularColorList, ChinazoCard, PhaseSet, SavedSet>(command);
        TempSets.HowManySets = 5;
    }
    public DeckObservablePile<ChinazoCard> Deck1 { get; set; }
    public SingleObservablePile<ChinazoCard> Pile1 { get; set; }
    public HandObservable<ChinazoCard> PlayerHand1 { get; set; }
    public SingleObservablePile<ChinazoCard>? OtherPile { get; set; }
    public TempSetsObservable<EnumSuitList, EnumRegularColorList, ChinazoCard> TempSets;
    public MainSetsObservable<EnumSuitList, EnumRegularColorList, ChinazoCard, PhaseSet, SavedSet> MainSets;
}