namespace SequenceDice.Core.ViewModels;
[InstanceGame]
public partial class SequenceDiceMainViewModel : BoardDiceGameVM
{
    private readonly SequenceDiceMainGameClass _mainGame; //if we don't need, delete.
    private readonly IToast _toast;
    public SequenceDiceVMData VMData { get; set; }
    public SequenceDiceMainViewModel(CommandContainer commandContainer,
        SequenceDiceMainGameClass mainGame,
        SequenceDiceVMData viewModel,
        BasicData basicData,
        TestOptions test,
        IGamePackageResolver resolver,
        IStandardRollProcesses roller,
        IEventAggregator aggregator,
        IToast toast
        )
        : base(commandContainer, mainGame, basicData, test, resolver, roller, aggregator)
    {
        _mainGame = mainGame;
        VMData = viewModel;
        _toast = toast;
        CreateCommands(commandContainer);
    }
    partial void CreateCommands(CommandContainer command);
    public DiceCup<SimpleDice> GetCup => VMData.Cup!;
    public PlayerCollection<SequenceDicePlayerItem> PlayerList => _mainGame.PlayerList;
    public override bool CanRollDice()
    {
        return _mainGame.SaveRoot!.GameStatus != EnumGameStatusList.MovePiece;
    }
    public override async Task RollDiceAsync()
    {
        await base.RollDiceAsync();
    }
    public bool CanMakeMove => _mainGame.SaveRoot.GameStatus == EnumGameStatusList.MovePiece;
    [Command(EnumCommandCategory.Game)]
    public async Task MakeMoveAsync(SpaceInfoCP space)
    {
        if (_mainGame.SaveRoot.GameBoard.CanMakeMove(space) == false)
        {
            _toast.ShowUserErrorToast("Illegal Move");
            return;
        }
        if (_mainGame.BasicData.MultiPlayer)
        {
            await _mainGame.Network!.SendMoveAsync(space);
        }
        await _mainGame.MakeMoveAsync(space);
    }
}