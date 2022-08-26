namespace ThreeLetterFun.Core.BeginningClasses;
[SingletonGame]
[AutoReset]
public class CardsChosenProcesses : ICardsChosenProcesses
{
    private readonly BasicData _basicData;
    private readonly TestOptions _test;
    private readonly IAsyncDelayer _delayer;
    private readonly IEventAggregator _aggregator;
    private readonly ThreeLetterFunMainGameClass _mainGame;
    private readonly IShuffleTiles _shuffle;
    private readonly IGameNetwork? _network;
    public CardsChosenProcesses(BasicData basicData,
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
    async Task ICardsChosenProcesses.CardsChosenAsync(int howManyCards)
    {
        if (_basicData.Client == false)
        {
            await _network!.SendAllAsync("howmanycards", howManyCards);
        }
        else if (_test.NoAnimations == false)
        {
            await _delayer.DelayMilli(300);
        }
        await _aggregator.PublishAsync(new NextScreenEventModel(EnumNextScreen.Finished));
        if (_basicData.Client == true)
        {
            _network!.IsEnabled = true;
            return;
        }
        await _shuffle.StartShufflingAsync(_mainGame, howManyCards);
    }
}
