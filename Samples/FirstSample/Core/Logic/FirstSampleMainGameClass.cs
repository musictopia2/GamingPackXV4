namespace FirstSample.Core.Logic;
[SingletonGame]
public class FirstSampleMainGameClass(ISaveSinglePlayerClass thisState,
    IEventAggregator aggregator,
    IGamePackageResolver container,
    BasicData data,
    IMessageBox message,
    ISystemError error
        ) : IAggregatorContainer
{
    private readonly ISaveSinglePlayerClass _thisState = thisState;
    private readonly IMessageBox _message = message;
    private readonly ISystemError _error = error;
    internal FirstSampleSaveInfo _saveRoot = container.ReplaceObject<FirstSampleSaveInfo>();
    private bool _opened;
    internal bool _gameGoing;
    public IEventAggregator Aggregator { get; } = aggregator;
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
        data.GameDataLoading = false; //needs this otherwise, if doing new game, would be stuck forever (bad).
    }
    private async Task RestoreGameAsync()
    {
        _saveRoot = await _thisState.RetrieveSinglePlayerGameAsync<FirstSampleSaveInfo>();
    }
    public async Task ShowWinAsync()
    {
        _gameGoing = false;
        await _message.ShowMessageAsync("You Win");
        await _thisState.DeleteSinglePlayerGameAsync();
        //send message to show win.
        await this.SendGameOverAsync(_error);
    }
}