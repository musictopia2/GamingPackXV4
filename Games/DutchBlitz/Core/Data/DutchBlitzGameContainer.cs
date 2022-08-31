namespace DutchBlitz.Core.Data;
[SingletonGame]
[AutoReset] //usually needs reset
public class DutchBlitzGameContainer : CardGameContainer<DutchBlitzCardInformation, DutchBlitzPlayerItem, DutchBlitzSaveInfo>
{
    public DutchBlitzGameContainer(BasicData basicData,
        TestOptions test,
        IGameInfo gameInfo,
        IAsyncDelayer delay,
        IEventAggregator aggregator,
        CommandContainer command,
        IGamePackageResolver resolver,
        IListShuffler<DutchBlitzCardInformation> deckList,
        IRandomGenerator random) : base(basicData, test, gameInfo, delay, aggregator, command, resolver, deckList, random)
    {
    }
    public int MaxDiscard;
    public ComputerCards? CurrentComputer;
    public BasicList<ComputerCards> ComputerPlayers = new();
}