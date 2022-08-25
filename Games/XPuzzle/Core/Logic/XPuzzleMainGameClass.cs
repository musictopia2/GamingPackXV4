namespace XPuzzle.Core.Logic;
[SingletonGame]
public class XPuzzleMainGameClass : IAggregatorContainer
{
    private readonly ISaveSinglePlayerClass _thisState;
    private readonly IMessageBox _message;
    private readonly ISystemError _error;
    internal XPuzzleSaveInfo _saveRoot;
    public XPuzzleMainGameClass(ISaveSinglePlayerClass thisState,
        IEventAggregator aggregator,
        IGamePackageResolver container,
        IMessageBox message,
        ISystemError error
        )
    {
        _thisState = thisState;
        Aggregator = aggregator;
        _message = message;
        _error = error;
        _saveRoot = container.ReplaceObject<XPuzzleSaveInfo>(); //can't create new one.  because if doing that, then anything that needs it won't have it.
    }
    private bool _opened;
    internal bool _gameGoing;
    public IEventAggregator Aggregator { get; }
    public async Task NewGameAsync()
    {
        _gameGoing = true;
        if (_opened == false)
        {
            _opened = true;
            if (await _thisState.CanOpenSavedSinglePlayerGameAsync())
            {
                await RestoreGameAsync();
                return;
            }
        }
    }
    private async Task RestoreGameAsync()
    {
        _saveRoot = await _thisState.RetrieveSinglePlayerGameAsync<XPuzzleSaveInfo>();
    }
    public async Task ShowWinAsync()
    {
        _gameGoing = false;
        await _message.ShowMessageAsync("You Win");
        await _thisState.DeleteSinglePlayerGameAsync();
        await this.SendGameOverAsync(_error);
    }
}