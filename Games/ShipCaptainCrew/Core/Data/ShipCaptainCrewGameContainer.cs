namespace ShipCaptainCrew.Core.Data;
[SingletonGame]
public class ShipCaptainCrewGameContainer(BasicData basicData,
    TestOptions test,
    IGameInfo gameInfo,
    IAsyncDelayer delay,
    IEventAggregator aggregator,
    CommandContainer command,
    IGamePackageResolver resolver,
    IRandomGenerator random) : BasicGameContainer<ShipCaptainCrewPlayerItem, ShipCaptainCrewSaveInfo>(basicData, test, gameInfo, delay, aggregator, command, resolver, random)
{
}