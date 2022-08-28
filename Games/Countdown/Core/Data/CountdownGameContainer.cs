namespace Countdown.Core.Data;
[SingletonGame]
public class CountdownGameContainer : BasicGameContainer<CountdownPlayerItem, CountdownSaveInfo>
{
    public CountdownGameContainer(BasicData basicData,
        TestOptions test,
        IGameInfo gameInfo,
        IAsyncDelayer delay,
        IEventAggregator aggregator,
        CommandContainer command,
        IGamePackageResolver resolver,
        IRandomGenerator random) : base(basicData, test, gameInfo, delay, aggregator, command, resolver, random)
    {
    }
    public Func<SimpleNumber, Task>? MakeMoveAsync { get; set; }
    public Func<BasicList<SimpleNumber>>? GetNumberList { get; set; }
    public BasicList<CountdownPlayerItem> GetPlayerList()
    {
        //this will return a list starting with self.
        if (BasicData.MultiPlayer == false)
        {
            return SaveRoot.PlayerList.ToBasicList();
        }
        return SaveRoot.PlayerList.GetAllPlayersStartingWithSelf();
    }
}