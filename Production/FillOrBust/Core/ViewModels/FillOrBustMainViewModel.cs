namespace FillOrBust.Core.ViewModels;
[InstanceGame]
public partial class FillOrBustMainViewModel(CommandContainer commandContainer,
        FillOrBustMainGameClass mainGame,
        FillOrBustVMData viewModel,
        BasicData basicData,
        IGamePackageResolver resolver,
        IEventAggregator aggregator,
        IToast toast) : BasicCardGamesVM<FillOrBustCardInformation>(commandContainer, mainGame, viewModel, basicData, resolver, aggregator, toast)
{
    protected override void AdditionalInitialize()
    {
        viewModel.Deck1.NeverAutoDisable = true;
        viewModel.Cup!.SendEnableProcesses(this, () =>
        {
            return mainGame!.SaveRoot!.GameStatus == EnumGameStatusList.ChooseDice;
        });
        viewModel.Cup!.DiceClickedAsync = FillOrBustMainViewModel_DiceClickedAsync;
        CreateCommands(CommandContainer);
        base.AdditionalInitialize();
    }
    partial void CreateCommands(CommandContainer command);
    private async Task FillOrBustMainViewModel_DiceClickedAsync(SimpleDice arg)
    {
        await mainGame!.Roller!.SelectUnSelectDiceAsync(arg.Index); // i think
    }
    protected override bool CanEnableDeck()
    {
        return mainGame!.SaveRoot!.GameStatus == EnumGameStatusList.DrawCard ||
            mainGame.SaveRoot.GameStatus == EnumGameStatusList.ChoosePlay ||
            mainGame.SaveRoot.GameStatus == EnumGameStatusList.ChooseDraw;
    }
    protected override bool CanEnablePile1()
    {
        return false;
    }
    public override bool CanEndTurn()
    {
        if (mainGame!.SaveRoot!.GameStatus == EnumGameStatusList.EndTurn ||
            mainGame.SaveRoot.GameStatus == EnumGameStatusList.ChooseRoll ||
            mainGame.SaveRoot.GameStatus == EnumGameStatusList.ChooseDraw)
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
        if (mainGame!.SaveRoot!.GameStatus == EnumGameStatusList.RollDice ||
            mainGame.SaveRoot.GameStatus == EnumGameStatusList.ChooseRoll ||
            mainGame.SaveRoot.GameStatus == EnumGameStatusList.ChoosePlay)
        {
            return true;
        }
        return false;
    }
    [Command(EnumCommandCategory.Game)]
    public async Task RollDiceAsync()
    {
        await mainGame!.Roller!.RollDiceAsync();
    }
    public bool CanChooseDice => mainGame.SaveRoot.GameStatus == EnumGameStatusList.ChooseDice;
    [Command(EnumCommandCategory.Game)]
    public async Task ChooseDiceAsync()
    {
        int score = mainGame!.CalculateScore();
        if (score == 0)
        {
            Toast.ShowUserErrorToast("Sorry, you must choose at least one scoring dice");
            return;
        }
        if (mainGame.BasicData!.MultiPlayer == true)
        {
            await mainGame.Network!.SendAllAsync("updatescore", score);
        }
        await mainGame.AddToTempAsync(score);
    }
}