namespace SorryCardGame.Blazor.Views;
public partial class SorryCardGameMainView
{
    private readonly BasicList<LabelGridModel> _labels = [];
    private SorryCardGameVMData? _vmData;
    private SorryCardGameGameContainer? _gameContainer;
    private BasicList<SorryCardGamePlayerItem> _playerList = [];
    protected override void OnInitialized()
    {
        _vmData = Resolver!.Resolve<SorryCardGameVMData>();
        _gameContainer = Resolver.Resolve<SorryCardGameGameContainer>();
        _labels.AddLabel("Turn", nameof(SorryCardGameVMData.NormalTurn))
            .AddLabel("Status", nameof(SorryCardGameVMData.Status))
            .AddLabel("Instructions", nameof(SorryCardGameVMData.Instructions));
        _playerList = _gameContainer.PlayerList!.GetAllPlayersStartingWithSelf();
        base.OnInitialized();
    }
}