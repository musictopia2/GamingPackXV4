namespace YaBlewIt.Core.Data;
[SingletonGame]
[UseLabelGrid]
[AutoReset]
public partial class YaBlewItVMData : IBasicCardGamesData<YaBlewItCardInformation>, ICup<EightSidedDice>
{
    private readonly CommandContainer _command;
    private readonly IGamePackageResolver _resolver;
    [LabelColumn]
    public string NormalTurn { get; set; } = "";
    [LabelColumn]
    public string OtherLabel { get; set; } = "";
    [LabelColumn]
    public string Status { get; set; } = "";
    [LabelColumn]
    public string Instructions { get; set; } = "";
    public YaBlewItVMData(CommandContainer command, IGamePackageResolver resolver)
    {
        Deck1 = new(command);
        Pile1 = new(command);
        PlayerHand1 = new(command);
        Claims = new(command);
        Claims.Text = "Claims";
        _command = command;
        _resolver = resolver;
        ColorPicker = new(command, new SafeColorListClass())
        {
            AutoSelectCategory = EnumAutoSelectCategory.AutoEvent
        };
    }
    public DeckObservablePile<YaBlewItCardInformation> Deck1 { get; set; }
    public SingleObservablePile<YaBlewItCardInformation> Pile1 { get; set; }
    public HandObservable<YaBlewItCardInformation> PlayerHand1 { get; set; }
    public HandObservable<YaBlewItCardInformation> Claims { get; set; }
    public SingleObservablePile<YaBlewItCardInformation>? OtherPile { get; set; }
    public SimpleEnumPickerVM<EnumColors> ColorPicker;
    public void LoadCup(YaBlewItSaveInfo saveRoot, bool autoResume)
    {
        if (Cup != null && autoResume)
        {
            return;
        }
        Cup = new(saveRoot.DiceList, _resolver, _command);
        Cup.HowManyDice = 1;
        Cup.CanShowDice = autoResume; //well see.
    }
    public DiceCup<EightSidedDice>? Cup { get; set; }
}