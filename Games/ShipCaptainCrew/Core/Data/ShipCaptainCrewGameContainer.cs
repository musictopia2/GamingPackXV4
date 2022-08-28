namespace ShipCaptainCrew.Core.Data;
[SingletonGame]
public class ShipCaptainCrewGameContainer : BasicGameContainer<ShipCaptainCrewPlayerItem, ShipCaptainCrewSaveInfo>
{
    public ShipCaptainCrewGameContainer(BasicData basicData,
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