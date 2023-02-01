namespace Risk.Blazor;
public partial class GameBoardBlazor
{
    private readonly RiskGameContainer _gameContainer;
    public GameBoardBlazor()
    {
        _gameContainer = aa1.Resolver!.Resolve<RiskGameContainer>();
    }
    [Parameter]
    public BasicList<TerritoryModel> Territories { get; set; } = new();
    [Parameter]
    public string TargetHeight { get; set; } = "85vh"; //default at this but can change.
    [Parameter]
    public EventCallback<TerritoryModel> OnTerritoryClicked { get; set; }
    private BasicList<TerritoryModel> UnselectedList => Territories.GetUnselectedTerritories(_gameContainer);
    private BasicList<TerritoryModel> SelectedList => Territories.GetSelectedTerritories(_gameContainer);
}