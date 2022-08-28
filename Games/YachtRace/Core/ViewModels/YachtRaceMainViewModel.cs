namespace YachtRace.Core.ViewModels;
[InstanceGame]
public partial class YachtRaceMainViewModel : DiceGamesVM<SimpleDice>
{
    private readonly YachtRaceMainGameClass _mainGame; //if we don't need, delete.
    public YachtRaceVMData VMData { get; set; }
    public YachtRaceMainViewModel(CommandContainer commandContainer,
        YachtRaceMainGameClass mainGame,
        YachtRaceVMData viewModel,
        BasicData basicData,
        TestOptions test,
        IGamePackageResolver resolver,
        IStandardRollProcesses roller,
        IEventAggregator aggregator
        )
        : base(commandContainer, mainGame, viewModel, basicData, test, resolver, roller, aggregator)
    {
        _mainGame = mainGame;
        VMData = viewModel;
        CommandContainer.ExecutingChanged += CommandContainer_ExecutingChanged;
        CreateCommands(commandContainer);
    }
    partial void CreateCommands(CommandContainer command);
    protected override Task TryCloseAsync()
    {
        CommandContainer.ExecutingChanged -= CommandContainer_ExecutingChanged;
        return base.TryCloseAsync();
    }
    private void CommandContainer_ExecutingChanged()
    {
        if (_mainGame.HasRolled && CommandContainer.IsExecuting)
        {
            try
            {
                VMData.Stops.Stop();
            }
            catch (Exception)
            {

                throw;
            }
        }
        else if (CommandContainer.IsExecuting == false && _mainGame.HasRolled)
        {
            VMData.Stops.Start();
        }
    }
    public DiceCup<SimpleDice> GetCup => VMData.Cup!;
    public PlayerCollection<YachtRacePlayerItem> PlayerList => _mainGame.PlayerList;
    protected override bool CanEnableDice()
    {
        return true;
    }
    public override bool CanEndTurn()
    {
        return false;
    }
    public override bool CanRollDice()
    {
        return true;
    }
    public bool CanFiveKind => _mainGame.HasRolled;
    [Command(EnumCommandCategory.Game)]
    public async Task FiveKindAsync()
    {
        if (_mainGame!.HasYahtzee() == false)
        {
            VMData.ErrorMessage = "You do not have 5 of a kind";
            CommandContainer.UpdateAll();
            await Task.Delay(200);
            VMData.ErrorMessage = "";
            CommandContainer.UpdateAll();
            return;
        }
        VMData.Stops.Stop();
        float howLong = VMData.Stops.ElapsedMilliseconds; //hopefully this simple.
        if (howLong == 0)
        {
            throw new CustomBasicException("Time cannot be 0");
        }
        if (_mainGame.BasicData!.MultiPlayer == true)
        {
            await _mainGame.Network!.SendAllAsync("fivekind", howLong);
        }
        await _mainGame.ProcessFiveKindAsync(howLong);
    }
}