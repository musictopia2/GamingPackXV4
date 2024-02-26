namespace BasicGameFrameworkLibrary.Core.SpecializedGameTypes.YahtzeeStyleHelpers.Containers;
public class YahtzeeGameContainer<D> : BasicGameContainer<YahtzeePlayerItem<D>, YahtzeeSaveInfo<D>>
    where D : SimpleDice, new()
{
#pragma warning disable IDE0290 // Use primary constructor
    public YahtzeeGameContainer(BasicData basicData,
#pragma warning restore IDE0290 // Use primary constructor
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