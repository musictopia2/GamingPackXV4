namespace YahtzeeHandsDown.Core.ViewModels;
[InstanceGame]
public partial class YahtzeeHandsDownMainViewModel(CommandContainer commandContainer,
        YahtzeeHandsDownMainGameClass mainGame,
        YahtzeeHandsDownVMData viewModel,
        BasicData basicData,
        IGamePackageResolver resolver,
        YahtzeeHandsDownGameContainer gameContainer,
        IEventAggregator aggregator,
        IToast toast) : BasicCardGamesVM<YahtzeeHandsDownCardInformation>(commandContainer, mainGame, viewModel, basicData, resolver, aggregator, toast)
{
    protected override void AdditionalInitialize()
    {
        viewModel.Deck1.NeverAutoDisable = true;
        viewModel.PlayerHand1.Maximum = 5;
        viewModel.PlayerHand1.AutoSelect = EnumHandAutoType.SelectAsMany;
        CreateCommands(CommandContainer);
        base.AdditionalInitialize();
    }
    partial void CreateCommands(CommandContainer command);
    protected override bool CanEnableDeck()
    {
        return false;
    }
    protected override bool CanEnablePile1()
    {
        if (mainGame!.PlayerList.Count > 2)
        {
            return !gameContainer.AlreadyDrew;
        }
        if (mainGame.SaveRoot!.ExtraTurns < 4)
        {
            return !gameContainer.AlreadyDrew;
        }
        return false;
    }
    protected override async Task ProcessDiscardClickedAsync()
    {
        var thisList = viewModel.PlayerHand1!.ListSelectedObjects();
        if (thisList.Count == 0)
        {
            Toast.ShowUserErrorToast("Must choose at least one card to discard to get new cards");
            return;
        }
        if (mainGame.BasicData!.MultiPlayer)
        {
            var nextList = thisList.GetDeckListFromObjectList();
            await mainGame.Network!.SendAllAsync("replacecards", nextList);
        }
        await mainGame!.ReplaceCardsAsync(thisList);
    }
    public override bool CanEnableAlways()
    {
        return true;
    }
    public override bool CanEndTurn()
    {
        return !CanEnablePile1();
    }
    [Command(EnumCommandCategory.Game)]
    public async Task GoOutAsync()
    {
        var results = mainGame!.GetResults();
        if (results.Count == 0)
        {
            Toast.ShowUserErrorToast("Cannot go out");
            return;
        }
        if (mainGame.BasicData!.MultiPlayer)
        {
            await mainGame.Network!.SendAllAsync("wentout", results.First());
        }
        await mainGame.PlayerGoesOutAsync(results.First());
    }
}