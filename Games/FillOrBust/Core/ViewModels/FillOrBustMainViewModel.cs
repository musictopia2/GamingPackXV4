namespace FillOrBust.Core.ViewModels;
[InstanceGame]
public partial class FillOrBustMainViewModel : BasicCardGamesVM<FillOrBustCardInformation>
{
    private readonly FillOrBustMainGameClass _mainGame;
    private readonly FillOrBustVMData _model;
    private readonly IToast _toast;
    public FillOrBustMainViewModel(CommandContainer commandContainer,
        FillOrBustMainGameClass mainGame,
        FillOrBustVMData viewModel,
        BasicData basicData,
        TestOptions test,
        IGamePackageResolver resolver,
        IEventAggregator aggregator,
        IToast toast
        )
        : base(commandContainer, mainGame, viewModel, basicData, test, resolver, aggregator, toast)
    {
        _mainGame = mainGame;
        _model = viewModel;
        _toast = toast;
        _model.Deck1.NeverAutoDisable = true;
        _model.Cup!.SendEnableProcesses(this, () =>
        {
            return _mainGame!.SaveRoot!.GameStatus == EnumGameStatusList.ChooseDice;
        });
        _model.Cup!.DiceClickedAsync = FillOrBustMainViewModel_DiceClickedAsync;
        CreateCommands(commandContainer);
    }
    partial void CreateCommands(CommandContainer command);
    private async Task FillOrBustMainViewModel_DiceClickedAsync(SimpleDice arg)
    {
        await _mainGame!.Roller!.SelectUnSelectDiceAsync(arg.Index); // i think
    }
    protected override bool CanEnableDeck()
    {
        return _mainGame!.SaveRoot!.GameStatus == EnumGameStatusList.DrawCard ||
            _mainGame.SaveRoot.GameStatus == EnumGameStatusList.ChoosePlay ||
            _mainGame.SaveRoot.GameStatus == EnumGameStatusList.ChooseDraw;
    }
    protected override bool CanEnablePile1()
    {
        return false;
    }
    public override bool CanEndTurn()
    {
        if (_mainGame!.SaveRoot!.GameStatus == EnumGameStatusList.EndTurn ||
            _mainGame.SaveRoot.GameStatus == EnumGameStatusList.ChooseRoll ||
            _mainGame.SaveRoot.GameStatus == EnumGameStatusList.ChooseDraw)
        {
            return true;
        }
        return false;
    }
    protected override async Task ProcessDiscardClickedAsync()
    {
        await Task.CompletedTask;
    }
    public override bool CanEnableAlways()
    {
        return true;
    }
    public bool CanRollDice()
    {
        if (_mainGame!.SaveRoot!.GameStatus == EnumGameStatusList.RollDice ||
            _mainGame.SaveRoot.GameStatus == EnumGameStatusList.ChooseRoll ||
            _mainGame.SaveRoot.GameStatus == EnumGameStatusList.ChoosePlay)
        {
            return true;
        }
        return false;
    }
    [Command(EnumCommandCategory.Game)]
    public async Task RollDiceAsync()
    {
        await _mainGame!.Roller!.RollDiceAsync();
    }
    public bool CanChooseDice => _mainGame.SaveRoot.GameStatus == EnumGameStatusList.ChooseDice;
    [Command(EnumCommandCategory.Game)]
    public async Task ChooseDiceAsync()
    {
        int score = _mainGame!.CalculateScore();
        if (score == 0)
        {
            _toast.ShowUserErrorToast("Sorry, you must choose at least one scoring dice");
            return;
        }
        if (_mainGame.BasicData!.MultiPlayer == true)
        {
            await _mainGame.Network!.SendAllAsync("updatescore", score);
        }
        await _mainGame.AddToTempAsync(score);
    }
}