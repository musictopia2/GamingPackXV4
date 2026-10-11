namespace BowlingDiceGame.Blazor;
public partial class BowlingDiceListBlazor : IDisposable
{
    [Parameter]
    public BasicList<SingleDiceInfo>? DiceList { get; set; }
    private readonly CommandContainer _command;
    public BowlingDiceListBlazor()
    {
        _command = Resolver!.Resolve<CommandContainer>();
    }
    protected override void OnInitialized()
    {
        _command.AddSpecificAction(BowlingDiceSet.CommandActionString, ShowChange);
        base.OnInitialized();
    }
    private async void ShowChange()
    {
        await InvokeAsync(StateHasChanged);
    }
    public void Dispose()
    {
        _command.RemoveSpecificAction(BowlingDiceSet.CommandActionString);
        GC.SuppressFinalize(this);
    }
}