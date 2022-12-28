namespace Phase10.Core.Data;
[SingletonGame]
[UseLabelGrid]
[AutoReset]
public partial class Phase10VMData : IBasicCardGamesData<Phase10CardInformation>
{
    [LabelColumn]
    public string NormalTurn { get; set; } = "";
    [LabelColumn]
    public string Status { get; set; } = "";
    [LabelColumn]
    public string CurrentPhase { get; set; } = "";
    public Phase10VMData(CommandContainer command, IGamePackageResolver resolver)
    {
        Deck1 = new(command);
        Pile1 = new(command);
        PlayerHand1 = new(command);
        PlayerHand1.Maximum = 11;
        TempSets = new TempSetsObservable<EnumColorTypes, EnumColorTypes, Phase10CardInformation>(command, resolver);
        MainSets = new MainSetsObservable<EnumColorTypes, EnumColorTypes, Phase10CardInformation, PhaseSet, SavedSet>(command);
        TempSets.HowManySets = 5;
        PlayerPicker = new(command, resolver);
        PlayerPicker.IndexMethod = EnumIndexMethod.OneBased;
        PlayerPicker.ItemSelectedAsync = PlayerPicker_ItemSelectedAsync;
    }
    public DeckObservablePile<Phase10CardInformation> Deck1 { get; set; }
    public SingleObservablePile<Phase10CardInformation> Pile1 { get; set; }
    public HandObservable<Phase10CardInformation> PlayerHand1 { get; set; }
    public SingleObservablePile<Phase10CardInformation>? OtherPile { get; set; }
    public TempSetsObservable<EnumColorTypes, EnumColorTypes, Phase10CardInformation> TempSets;
    public MainSetsObservable<EnumColorTypes, EnumColorTypes, Phase10CardInformation, PhaseSet, SavedSet> MainSets;
    public ListViewPicker PlayerPicker { get; set; }
    public string PlayerChosen { get; set; } = "";
    private Task PlayerPicker_ItemSelectedAsync(int SelectedIndex, string SelectedText)
    {
        PlayerChosen = SelectedText;
        return Task.CompletedTask;
    }

}