namespace BowlingDiceGame.Blazor;
public partial class BowlingDiceListBlazor
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
        _command.AddAction(ShowChange, BowlingDiceSet.CommandActionString);
        base.OnInitialized();
    }
    private async void ShowChange()
    {
        await InvokeAsync(StateHasChanged);
    }

}