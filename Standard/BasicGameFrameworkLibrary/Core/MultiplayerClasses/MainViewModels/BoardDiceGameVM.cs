namespace BasicGameFrameworkLibrary.Core.MultiplayerClasses.MainViewModels;
public abstract partial class BoardDiceGameVM : BasicMultiplayerMainVM
{
    private readonly IStandardRollProcesses _rollProcesses;
    public BoardDiceGameVM(CommandContainer commandContainer,
        IEndTurn mainGame,
        BasicData basicData,
        TestOptions test,
        IGamePackageResolver resolver,
        IStandardRollProcesses rollProcesses,
        IEventAggregator aggregator
        ) : base(commandContainer, mainGame, basicData, test, resolver, aggregator)
    {
        _rollProcesses = rollProcesses;
        CreateCommands(commandContainer);
    }
    partial void CreateCommands(CommandContainer command);
    public virtual bool CanRollDice()
    {
        return true;  //can be false in some cases (?)
    }
    [Command(EnumCommandCategory.Game)]
    public virtual async Task RollDiceAsync() //some games require something else.
    {
        await _rollProcesses.RollDiceAsync();
    }
}