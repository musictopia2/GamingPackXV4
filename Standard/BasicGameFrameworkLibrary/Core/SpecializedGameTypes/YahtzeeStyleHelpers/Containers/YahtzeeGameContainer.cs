namespace BasicGameFrameworkLibrary.Core.SpecializedGameTypes.YahtzeeStyleHelpers.Containers;
public class YahtzeeGameContainer<D>(BasicData basicData,
    TestOptions test,
    IGameInfo gameInfo,
    IAsyncDelayer delay,
    IEventAggregator aggregator,
    CommandContainer command,
    IGamePackageResolver resolver,
    IRandomGenerator random) : BasicGameContainer<YahtzeePlayerItem<D>, YahtzeeSaveInfo<D>>(basicData, test, gameInfo, delay, aggregator, command, resolver, random)
    where D : SimpleDice, new()
{
    public Func<Task>? GetNewScoreAsync { get; set; }
}