namespace Risk.Blazor;
public partial class TerritoryBlazor
{
    [Parameter]
    public TerritoryModel? Territory { get; set; }
    [Parameter]
    public EventCallback<TerritoryModel> OnClick { get; set; }
    private readonly RiskGameContainer _gameContainer;
    public TerritoryBlazor()
    {
        _gameContainer = aa.Resolver!.Resolve<RiskGameContainer>();
    }
    private async Task PrivateClick()
    {
        if (_gameContainer.Command.IsExecuting)
        {
            return;
        }
        if (_gameContainer.SaveRoot.Stage == EnumStageList.EndTurn || _gameContainer.SaveRoot.Stage == EnumStageList.None || _gameContainer.SaveRoot.Stage == EnumStageList.TransferAfterBattle || _gameContainer.SaveRoot.Stage == EnumStageList.Begin)
        {
            return; //since you need to end turn, you can see the board but you cannot click anywhere on the board.
        }
        await _gameContainer.Command.ProcessCustomCommandAsync(async () =>
        {
            await OnClick.InvokeAsync(Territory);
        });
    }
    private static SizeF GetSize => new(20, 20); //well see what i can do (?)
}