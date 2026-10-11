namespace SorryDicedGame.Blazor;
public partial class CompleteDiceRollerComponent : IDisposable
{
    [CascadingParameter]
    public SorryDicedGameMainViewModel? DataContext { get; set; }
    [Parameter]
    [EditorRequired]
    public string ImageHeight { get; set; } = "";
    [Parameter]
    [EditorRequired]
    public ICustomCommand? DiceCommand { get; set; }
    private BasicList<SorryDiceModel> DiceList { get; set; } = [];
    protected override void OnInitialized()
    {
        DataContext!.CommandContainer.AddSpecificAction("sorrydice", ShowChange);
        base.OnInitialized();
    }
    protected override void OnParametersSet()
    {
        DiceList = DataContext!.MainGame.SaveRoot.DiceList;
        base.OnParametersSet();
    }
    private void ShowChange()
    {
        InvokeAsync(StateHasChanged);
    }
    public void Dispose()
    {
        DataContext!.CommandContainer.RemoveSpecificAction("sorrydice");
        GC.SuppressFinalize(this);
    }
}