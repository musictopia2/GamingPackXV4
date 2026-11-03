namespace YachtRace.Core.ViewModels;
[InstanceGame]
public partial class YachtRaceMainViewModel(CommandContainer commandContainer,
        YachtRaceMainGameClass mainGame,
        YachtRaceVMData viewModel,
        BasicData basicData,
        IGamePackageResolver resolver,
        IStandardRollProcesses roller,
        IEventAggregator aggregator) : DiceGamesVM<SimpleDice>(commandContainer, mainGame, viewModel, basicData, resolver, roller, aggregator)
{
    public YachtRaceVMData VMData { get; set; } = viewModel;
    protected override void AdditionalInitialize()
    {
        CommandContainer.ExecutingChanged = CommandContainer_ExecutingChanged;
        CreateCommands(CommandContainer);
        base.AdditionalInitialize();
    }
    protected override Task CloseAsync()
    {
        CommandContainer.ExecutingChanged = null;
        return base.CloseAsync();
    }
    partial void CreateCommands(CommandContainer command);
    private void CommandContainer_ExecutingChanged()
    {
        if (mainGame.HasRolled && CommandContainer.IsExecuting)
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
        else if (CommandContainer.IsExecuting == false && mainGame.HasRolled)
        {
            VMData.Stops.Start();
        }
    }
    public DiceCup<SimpleDice> GetCup => VMData.Cup!;
    public PlayerCollection<YachtRacePlayerItem> PlayerList => mainGame.PlayerList;
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
    public bool CanFiveKind => mainGame.HasRolled;
    [Command(EnumCommandCategory.Game)]
    public async Task FiveKindAsync()
    {
        if (mainGame!.HasYahtzee() == false)
        {
            VMData.ErrorMessage = "You do not have 5 of a kind";
            await Task.Delay(200);
            VMData.ErrorMessage = "";
            return;
        }
        VMData.Stops.Stop();
        float howLong = VMData.Stops.ElapsedMilliseconds; //hopefully this simple.
        if (howLong == 0)
        {
            throw new CustomBasicException("Time cannot be 0");
        }
        if (mainGame.BasicData!.MultiPlayer == true)
        {
            await mainGame.Network!.SendAllAsync("fivekind", howLong);
        }
        await mainGame.ProcessFiveKindAsync(howLong);
    }
}