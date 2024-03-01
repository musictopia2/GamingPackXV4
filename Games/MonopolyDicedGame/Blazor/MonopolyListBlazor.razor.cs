namespace MonopolyDicedGame.Blazor;
public partial class MonopolyListBlazor
{
    [CascadingParameter]
    public MonopolyDicedGameMainViewModel? DataContext { get; set; }
    [Inject]
    private IToast? Toast { get; set; }
    private BasicList<BasicDiceModel> DiceList { get; set; } = [];
    [Parameter]
    [EditorRequired]
    public string ImageHeight { get; set; } = "";
    protected override void OnInitialized()
    {
        DataContext!.CommandContainer.AddAction(ShowChange, "monopolydice");
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
    private async Task SelectDiceAsync(BasicDiceModel dice)
    {
        if (DataContext!.MainGame.SaveRoot.NumberOfCops > 2)
        {
            Toast!.ShowUserErrorToast("Cannot choose dice because you already have 3 cops");
            return;
        }
        await DataContext.MonopolyDice.SelectDiceAsync(dice);
    }
}