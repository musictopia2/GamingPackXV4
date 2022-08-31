namespace SorryCardGame.Blazor.Views;
public partial class SorryCardGameMainView
{
    private readonly BasicList<LabelGridModel> _labels = new();
    private SorryCardGameVMData? _vmData;
    private SorryCardGameGameContainer? _gameContainer;
    private BasicList<SorryCardGamePlayerItem> _playerList = new();
    protected override void OnInitialized()
    {
        _vmData = aa.Resolver!.Resolve<SorryCardGameVMData>();
        _gameContainer = aa.Resolver.Resolve<SorryCardGameGameContainer>();
        _labels.AddLabel("Turn", nameof(SorryCardGameVMData.NormalTurn))
            .AddLabel("Status", nameof(SorryCardGameVMData.Status))
            .AddLabel("Instructions", nameof(SorryCardGameVMData.Instructions));
        _playerList = _gameContainer.PlayerList!.GetAllPlayersStartingWithSelf();
        base.OnInitialized();
    }
}