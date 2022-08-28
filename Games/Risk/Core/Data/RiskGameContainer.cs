namespace Risk.Core.Data;
[SingletonGame]
[AutoReset]
public class RiskGameContainer : BasicGameContainer<RiskPlayerItem, RiskSaveInfo>
{
    public RiskGameContainer(BasicData basicData,
        TestOptions test,
        IGameInfo gameInfo,
        IAsyncDelayer delay,
        IEventAggregator aggregator,
        CommandContainer command,
        IGamePackageResolver resolver,
        IRandomGenerator random,
        RiskVMData data
        ) : base(basicData, test, gameInfo, delay, aggregator, command, resolver, random)
    {
        VMData = data;
    }
    public Func<Task>? AfterRollingAsync { get; set; }
    public Func<SendAttackResult, Task>? SentAttackProcessesAsync { get; set; }
    public Action? StartSentAttack { get; set; }
    public Func<Task>? EndMoveAsync { get; set; }
    public RiskVMData VMData { get; }
}