namespace BasicGameFrameworkLibrary.Core.SpecializedGameTypes.YahtzeeStyleHelpers.Containers;

public class YahtzeeGameContainer<D> : BasicGameContainer<YahtzeePlayerItem<D>, YahtzeeSaveInfo<D>>
    where D : SimpleDice, new()
{
    public YahtzeeGameContainer(BasicData basicData,
        TestOptions test,
        IGameInfo gameInfo,
        IAsyncDelayer delay,
        IEventAggregator aggregator,
        CommandContainer command,
        IGamePackageResolver resolver,
        IRandomGenerator random) : base(basicData, test, gameInfo, delay, aggregator, command, resolver, random)
    {
    }
    public Func<Task>? GetNewScoreAsync { get; set; }
}