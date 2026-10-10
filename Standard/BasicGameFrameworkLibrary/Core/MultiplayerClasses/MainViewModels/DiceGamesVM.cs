namespace BasicGameFrameworkLibrary.Core.MultiplayerClasses.MainViewModels;
public abstract partial class DiceGamesVM<D>(CommandContainer commandContainer,
        IHoldUnholdProcesses mainGame,
        IBasicDiceGamesData<D> viewModel,
        BasicData basicData,
        IGamePackageResolver resolver,
        IStandardRollProcesses rollProcesses,
        IEventAggregator aggregator) : BasicMultiplayerMainVM(commandContainer, mainGame, basicData, resolver, aggregator)
    where D : IStandardDice, new()
{   
    protected override void AdditionalInitialize()
    {
        if (viewModel.Cup == null)
        {
            throw new CustomBasicException("There was no cup.  Rethink");
        }
        viewModel.Cup.SendEnableProcesses(this, CanEnableDice);
        viewModel.Cup.DiceClickedAsync = Cup_DiceClickedAsync;
        CreateCommands(CommandContainer);
        base.AdditionalInitialize();
    }
    partial void CreateCommands(CommandContainer command);
    private async Task Cup_DiceClickedAsync(D arg)
    {
        if (viewModel.Cup!.ShowHold)
        {
            await mainGame.HoldUnholdDiceAsync(arg.Index);
        }
        else
        {
            await rollProcesses.SelectUnSelectDiceAsync(arg.Index);
        }
    }
    public virtual bool CanRollDice()
    {
        return true;
    }
    [Command(EnumCommandCategory.Game)]
    public async Task RollDiceAsync()
    {
        await rollProcesses.RollDiceAsync();
    }
    protected abstract bool CanEnableDice();
}