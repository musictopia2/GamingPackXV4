namespace BasicGameFrameworkLibrary.Core.MultiplayerClasses.MainViewModels;
public abstract partial class DiceGamesVM<D> : BasicMultiplayerMainVM
    where D : IStandardDice, new()
{
    public DiceGamesVM(CommandContainer commandContainer,
        IHoldUnholdProcesses mainGame,
        IBasicDiceGamesData<D> viewModel,
        BasicData basicData,
        TestOptions test,
        IGamePackageResolver resolver,
        IStandardRollProcesses rollProcesses,
        IEventAggregator aggregator
        ) : base(commandContainer,
            mainGame,
            basicData,
            test,
            resolver,
            aggregator
            )
    {
        _mainGame = mainGame;
        _model = viewModel;
        _rollProcesses = rollProcesses;
        if (_model.Cup == null)
        {
            throw new CustomBasicException("There was no cup.  Rethink");
        }
        _model.Cup.SendEnableProcesses(this, CanEnableDice);
        _model.Cup.DiceClickedAsync += Cup_DiceClickedAsync;
        CreateCommands(commandContainer);
    }
    partial void CreateCommands(CommandContainer command);
    private readonly IHoldUnholdProcesses _mainGame;
    private readonly IBasicDiceGamesData<D> _model;
    private readonly IStandardRollProcesses _rollProcesses;
    private async Task Cup_DiceClickedAsync(D arg)
    {
        if (_model.Cup!.ShowHold)
        {
            await _mainGame.HoldUnholdDiceAsync(arg.Index);
        }
        else
        {
            await _rollProcesses.SelectUnSelectDiceAsync(arg.Index);
        }
    }
    public virtual bool CanRollDice()
    {
        return true;
    }
    [Command(EnumCommandCategory.Game)]
    public async Task RollDiceAsync()
    {
        await _rollProcesses.RollDiceAsync();
    }
    protected override Task TryCloseAsync()
    {
        if (_model.Cup == null)
        {
            return Task.CompletedTask;
        }
        _model.Cup.DiceClickedAsync -= Cup_DiceClickedAsync;
        return base.TryCloseAsync();
    }
    protected abstract bool CanEnableDice();
}