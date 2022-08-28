namespace LifeBoardGame.Core.Data;
[SingletonGame]
[UseLabelGrid]
public partial class LifeBoardGameVMData : ISimpleBoardGamesData, IEnableAlways
{
    [LabelColumn]
    public string NormalTurn { get; set; } = "";
    [LabelColumn]
    public string Status { get; set; } = "";
    [LabelColumn]
    public string Instructions { get; set; } = "";
    public int CurrentSelected { get; set; }
    public EnumWhatStatus GameStatus { get; set; } = EnumWhatStatus.None;
    public string GameDetails { get; set; } = "";
    public HandObservable<LifeBaseCard> HandList;
    public SimpleEnumPickerVM<EnumGender> GenderChooser { get; set; }
    public string PlayerChosen { get; set; } = "";
    public ListViewPicker PlayerPicker { get; set; }
    public SingleObservablePile<LifeBaseCard> SinglePile { get; set; }
    public LifeBoardGameVMData(CommandContainer command, IGamePackageResolver resolver)
    {
        HandList = new HandObservable<LifeBaseCard>(command);
        GenderChooser = new(command, new ColorListChooser<EnumGender>());
        GenderChooser.AutoSelectCategory = EnumAutoSelectCategory.AutoEvent;
        PlayerPicker = new(command, resolver);
        PlayerPicker.IndexMethod = EnumIndexMethod.OneBased;
        PlayerPicker.ItemSelectedAsync += PlayerPicker_ItemSelectedAsync;
        SinglePile = new(command);
        SinglePile.SendAlwaysEnable(this); //hopefuly this simple.
        SinglePile.Text = "Card";
        SinglePile.CurrentOnly = true;
        HandList.Text = "None";
        HandList.IgnoreMaxRules = true;
    }
    private Task PlayerPicker_ItemSelectedAsync(int SelectedIndex, string SelectedText)
    {
        PlayerChosen = SelectedText;
        return Task.CompletedTask;
    }
    public int GetRandomCard => HandList.HandList.GetRandomItem().Deck;
    bool IEnableAlways.CanEnableAlways()
    {
        return false;
    }
}