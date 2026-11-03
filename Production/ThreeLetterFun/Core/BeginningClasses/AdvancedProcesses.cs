namespace ThreeLetterFun.Core.BeginningClasses;
[SingletonGame]
[AutoReset]
public class AdvancedProcesses : IAdvancedProcesses
{
    private readonly BasicData _basicData;
    private readonly TestOptions _test;
    private readonly IAsyncDelayer _delayer;
    private readonly IEventAggregator _aggregator;
    private readonly ThreeLetterFunMainGameClass _mainGame;
    private readonly IShuffleTiles _shuffle;
    private readonly IGameNetwork? _network;
    public AdvancedProcesses(BasicData basicData,
        TestOptions test,
        IAsyncDelayer delayer,
        IEventAggregator aggregator,
        ThreeLetterFunMainGameClass mainGame,
        IShuffleTiles shuffle
        )
    {
        _basicData = basicData;
        _test = test;
        _delayer = delayer;
        _aggregator = aggregator;
        _mainGame = mainGame;
        _shuffle = shuffle;
        _network = _basicData.GetNetwork();
    }
    async Task IAdvancedProcesses.ChoseAdvancedOptions(bool easy, bool shortGame)
    {
        if (_basicData.Client == false)
        {
            AdvancedSettingModel model = new()
            {
                IsEasy = easy,
                ShortGame = shortGame
            };
            await _network!.SendAllAsync("advancedsettings", model);
        }
        else if (_test.NoAnimations == false)
        {
            await _delayer.DelayMilli(500);
        }
        await _aggregator.PublishAsync(new NextScreenEventModel(EnumNextScreen.Finished));
        if (_basicData.Client == true)
        {
            _network!.IsEnabled = true;
            return;
        }
        _mainGame.SaveRoot.ShortGame = shortGame;
        if (easy)
        {
            _mainGame.SaveRoot.Level = Data.EnumLevel.Moderate;
        }
        else
        {
            _mainGame.SaveRoot.Level = Data.EnumLevel.Hard;
        }
        await _shuffle.StartShufflingAsync(_mainGame, 0);
    }
}