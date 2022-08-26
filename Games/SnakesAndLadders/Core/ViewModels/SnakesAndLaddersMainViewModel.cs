namespace SnakesAndLadders.Core.ViewModels;
[InstanceGame]
public partial class SnakesAndLaddersMainViewModel : BasicMultiplayerMainVM
{
    private readonly SnakesAndLaddersMainGameClass _mainGame; //if we don't need, delete.
    private readonly IToast _toast;
    public SnakesAndLaddersVMData VMData { get; set; }
    public SnakesAndLaddersMainViewModel(CommandContainer commandContainer,
        SnakesAndLaddersMainGameClass mainGame,
        BasicData basicData,
        TestOptions test,
        IGamePackageResolver resolver,
        IEventAggregator aggregator,
        SnakesAndLaddersVMData data,
        IToast toast
        )
        : base(commandContainer, mainGame, basicData, test, resolver, aggregator)
    {
        _mainGame = mainGame;
        VMData = data;
        _toast = toast;
        CreateCommands(commandContainer);
    }
    partial void CreateCommands(CommandContainer command);
    public BasicList<SnakesAndLaddersPlayerItem> GetPlayerList
    {
        get
        {
            BasicList<SnakesAndLaddersPlayerItem> output = _mainGame.PlayerList.ToBasicList();
            output.RemoveSpecificItem(_mainGame.SingleInfo!);
            output.Add(_mainGame.SingleInfo!);
            return output;
        }
    }
    public SnakesAndLaddersPlayerItem CurrentPlayer => _mainGame.SingleInfo!;
    public DiceCup<SimpleDice> GetCup => VMData.Cup!;
    public bool CanRollDice => !_mainGame.SaveRoot.HasRolled;
    [Command(EnumCommandCategory.Game)]
    public async Task RollDiceAsync()
    {
        await _mainGame.Roll.RollDiceAsync();
    }
    public bool CanMakeMove(int space)
    {
        if (space == 0)
        {
            return false;
        }
        return _mainGame.SaveRoot.HasRolled;
    }
    [Command(EnumCommandCategory.Game)]
    public async Task MakeMoveAsync(int space)
    {
        if (_mainGame.GameBoard1.IsValidMove(space) == false)
        {
            _toast.ShowUserErrorToast("Illegal Move");
            return;
        }
        await _mainGame.MakeMoveAsync(space);
    }
}