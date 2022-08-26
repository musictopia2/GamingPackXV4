namespace RummyDice.Core.Data;
[SingletonGame]
public class RummyDiceGameContainer : BasicGameContainer<RummyDicePlayerItem, RummyDiceSaveInfo>
{
    public RummyDiceGameContainer(BasicData basicData,
        TestOptions test,
        IGameInfo gameInfo,
        IAsyncDelayer delay,
        IEventAggregator aggregator,
        CommandContainer command,
        IGamePackageResolver resolver,
        IRandomGenerator random) : base(basicData, test, gameInfo, delay, aggregator, command, resolver, random)
    {
    }
}