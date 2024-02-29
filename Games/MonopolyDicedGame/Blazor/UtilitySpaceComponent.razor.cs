namespace MonopolyDicedGame.Blazor;
public partial class UtilitySpaceComponent
{
    [Parameter]
    public BasicList<OwnedModel> OwnList { get; set; } = [];
    [Parameter]
    public EventCallback<EnumUtilityType> UtilityClicked { get; set; } //we need to know which one was clicked.
    [Parameter]
    public string TargetImageHeight { get; set; } = "";
    [Parameter]
    [EditorRequired]
    public bool IsEnabled { get; set; }
    [Inject]
    private IToast? Toast { get; set; }
    private BasicList<OwnedModel> _filteredList = [];

    private void PossibleUtilityClick(EnumUtilityType utility)
    {
        if (IsEnabled == false)
        {
            return;
        }
        if (_filteredList.Any(x => x.Utility == utility))
        {
            Toast!.ShowUserErrorToast($"Already have {utility}");
            return;
        }
        UtilityClicked.InvokeAsync(utility);
    }

    protected override void OnParametersSet()
    {
        _filteredList = OwnList.Where(x => x.UsedOn == EnumBasicType.Utility).ToBasicList();
    }

    private bool HadWater()
    {
        return _filteredList.Any(x => x.Utility == EnumUtilityType.Water);
    }

    private bool HadElectric()
    {
        return _filteredList.Any(x => x.Utility == EnumUtilityType.Electric);
    }

    private BasicDiceModel GetWaterDiceUsed()
    {
        BasicDiceModel output = new();
        if (_filteredList.Any(x => x.Utility == EnumUtilityType.Water && x.WasChance))
        {
            output.UseChance();
        }
        else
        {
            output.UseUtility(EnumUtilityType.Water);
        }
        return output;
    }
    private BasicDiceModel GetElectricUsed()
    {
        BasicDiceModel output = new();
        if (_filteredList.Any(x => x.Utility == EnumUtilityType.Electric && x.WasChance))
        {
            output.UseChance();
        }
        else
        {
            output.UseUtility(EnumUtilityType.Electric);
        }
        return output;
    }

    private static string GetUtilityColor => cc1.Black.ToWebColor();
    private static string GetUtilityBorder => cc1.White.ToWebColor();


}