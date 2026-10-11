namespace SequenceDice.Core.ViewModels;
[InstanceGame]
public partial class SequenceDiceMainViewModel(CommandContainer commandContainer,
        SequenceDiceMainGameClass mainGame,
        SequenceDiceVMData viewModel,
        BasicData basicData,
        IGamePackageResolver resolver,
        IStandardRollProcesses roller,
        IEventAggregator aggregator,
        IToast toast) : BoardDiceGameVM(commandContainer, mainGame, basicData, resolver, roller, aggregator)
{
    public SequenceDiceVMData VMData { get; set; } = viewModel;
    protected override void AdditionalInitialize()
    {
        CreateCommands(CommandContainer);
        base.AdditionalInitialize();
    }
    partial void CreateCommands(CommandContainer command);
    public DiceCup<SimpleDice> GetCup => VMData.Cup!;
    public PlayerCollection<SequenceDicePlayerItem> PlayerList => mainGame.PlayerList;
    public override bool CanRollDice()
    {
        return mainGame.SaveRoot!.GameStatus != EnumGameStatusList.MovePiece;
    }
    public override async Task RollDiceAsync()
    {
        await base.RollDiceAsync();
    }
    public bool CanMakeMove => mainGame.SaveRoot.GameStatus == EnumGameStatusList.MovePiece;
    [Command(EnumCommandCategory.Game)]
    public async Task MakeMoveAsync(SpaceInfoCP space)
    {
        if (mainGame.SaveRoot.GameBoard.CanMakeMove(space) == false)
        {
            toast.ShowUserErrorToast("Illegal Move");
            return;
        }
        if (mainGame.BasicData.MultiPlayer)
        {
            await mainGame.Network!.SendMoveAsync(space);
        }
        await mainGame.MakeMoveAsync(space);
    }
}