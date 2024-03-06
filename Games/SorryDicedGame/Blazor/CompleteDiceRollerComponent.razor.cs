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
        DataContext!.CommandContainer.AddAction(ShowChange, "sorrydice");
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
#pragma warning disable CA1816 // Dispose methods should call SuppressFinalize
    public void Dispose()
#pragma warning restore CA1816 // Dispose methods should call SuppressFinalize
    {
        DataContext!.CommandContainer.RemoveAction("sorrydice");
    }
}