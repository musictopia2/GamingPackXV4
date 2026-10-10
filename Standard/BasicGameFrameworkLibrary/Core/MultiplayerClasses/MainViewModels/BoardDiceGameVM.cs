namespace BasicGameFrameworkLibrary.Core.MultiplayerClasses.MainViewModels;
public abstract partial class BoardDiceGameVM(CommandContainer commandContainer,
        IEndTurn mainGame,
        BasicData basicData,
        IGamePackageResolver resolver,
        IStandardRollProcesses rollProcesses,
        IEventAggregator aggregator) : BasicMultiplayerMainVM(commandContainer, mainGame, basicData, resolver, aggregator)
{
    protected override void AdditionalInitialize()
    {
        CreateCommands(CommandContainer);
        base.AdditionalInitialize();
    }
    partial void CreateCommands(CommandContainer command);
    public virtual bool CanRollDice()
    {
        return true;
    }
    [Command(EnumCommandCategory.Game)]
    public virtual async Task RollDiceAsync()
    {
        await rollProcesses.RollDiceAsync();
    }
}