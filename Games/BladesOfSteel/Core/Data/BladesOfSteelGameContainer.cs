namespace BladesOfSteel.Core.Data;
[SingletonGame]
[AutoReset] //usually needs reset
public class BladesOfSteelGameContainer : CardGameContainer<RegularSimpleCard, BladesOfSteelPlayerItem, BladesOfSteelSaveInfo>
{
    public BladesOfSteelGameContainer(BasicData basicData,
        TestOptions test,
        IGameInfo gameInfo,
        IAsyncDelayer delay,
        IEventAggregator aggregator,
        CommandContainer command,
        IGamePackageResolver resolver,
        IListShuffler<RegularSimpleCard> deckList,
        IRandomGenerator random) : base(basicData, test, gameInfo, delay, aggregator, command, resolver, deckList, random)
    {
    }
    internal int OtherTurn
    {
        get
        {
            return SaveRoot!.PlayOrder.OtherTurn;
        }
        set
        {
            SaveRoot!.PlayOrder.OtherTurn = value;
        }
    }
    internal Func<IBasicList<RegularSimpleCard>, EnumAttackGroup>? GetAttackStage { get; set; }
    internal Func<IBasicList<RegularSimpleCard>, EnumDefenseGroup>? GetDefenseStage { get; set; }
}