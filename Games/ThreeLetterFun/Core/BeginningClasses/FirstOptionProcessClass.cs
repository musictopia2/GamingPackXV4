namespace ThreeLetterFun.Core.BeginningClasses;
[SingletonGame]
public class FirstOptionProcessClass : IFirstOptionProcesses
{
    private readonly BasicData _basicData;
    private readonly TestOptions _test;
    private readonly IAsyncDelayer _delayer;
    private readonly IGamePackageResolver _resolver;
    private readonly IEventAggregator _aggregator;
    private readonly IGameNetwork? _network;
    public FirstOptionProcessClass(BasicData basicData,
        TestOptions test,
        IAsyncDelayer delayer,
        IGamePackageResolver resolver,
        IEventAggregator aggregator
        )
    {
        _basicData = basicData;
        _test = test;
        _delayer = delayer;
        _resolver = resolver;
        _aggregator = aggregator;
        _network = _basicData.GetNetwork();
    }
    async Task IFirstOptionProcesses.BeginningOptionSelectedAsync(EnumFirstOption firstOption)
    {
        if (_basicData.Client == false)
        {
            await _network!.SendAllAsync("firstoption", firstOption);
        }
        else if (_test.NoAnimations == false)
        {
            await _delayer.DelayMilli(300);
        }
        if (firstOption == EnumFirstOption.Beginner)
        {
            ThreeLetterFunSaveInfo saveRoot = _resolver.Resolve<ThreeLetterFunSaveInfo>();
            saveRoot.Level = EnumLevel.Easy;
            await _aggregator.PublishAsync(new NextScreenEventModel(EnumNextScreen.Cards));
            if (_basicData.Client == true)
            {
                _network!.IsEnabled = true;
            }
            return;
        }
        await _aggregator.PublishAsync(new NextScreenEventModel(EnumNextScreen.Advanced));
        if (_basicData.Client == true)
        {
            _network!.IsEnabled = true;
        }
    }
}