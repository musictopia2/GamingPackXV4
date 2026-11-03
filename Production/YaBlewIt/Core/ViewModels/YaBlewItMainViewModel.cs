namespace YaBlewIt.Core.ViewModels;
[InstanceGame]
public partial class YaBlewItMainViewModel(CommandContainer commandContainer,
        YaBlewItMainGameClass mainGame,
        YaBlewItGameContainer gameContainer,
        YaBlewItVMData viewModel,
        BasicData basicData,
        IGamePackageResolver resolver,
        IChooseColorProcesses colorProcesses,
        IEventAggregator aggregator,
        IToast toast) : BasicCardGamesVM<YaBlewItCardInformation>(commandContainer, mainGame, viewModel, basicData, resolver, aggregator, toast)
{
    public YaBlewItVMData VMData { get; set; } = viewModel;
    protected override void AdditionalInitialize()
    {
        VMData.ColorPicker.ItemClickedAsync = ColorPicker_ItemClickedAsync;
        CreateCommands(CommandContainer);
        base.AdditionalInitialize();
        //VMData.ColorPicker.SendEnableProcesses(
        //    this,
        //() => mainGame.SaveRoot.GameStatus == EnumGameStatus.Safe);
    }
    partial void CreateCommands(CommandContainer command);
    public bool CanRollDice()
    {
        if (mainGame.SaveRoot.GameStatus == EnumGameStatus.StartGambling
            || mainGame.SaveRoot.GameStatus == EnumGameStatus.FinishGambling
            || mainGame.SaveRoot.GameStatus == EnumGameStatus.MinerRolling
            || mainGame.SaveRoot.GameStatus == EnumGameStatus.ResolveFire)
        {
            return true; //you can decide to roll.
        }
        return false;
    }
    [Command(EnumCommandCategory.Game)]
    public async Task RollDiceAsync()
    {
        await mainGame!.Roller!.RollDiceAsync();
    }
    public override bool CanEndTurn()
    {
        if (mainGame.SaveRoot.GameStatus == EnumGameStatus.ProspectorContinues
            || mainGame.SaveRoot.GameStatus == EnumGameStatus.EndingTurn
            || mainGame.SaveRoot.GameStatus == EnumGameStatus.ProspectorStarts)
        {
            return true; //well see about other conditions.
        }
        return false;
    }
    public bool CanPass()
    {
        if (mainGame.SaveRoot.PlayedFaulty)
        {
            return false; //now you have to roll because you played it.
        }
        if (mainGame.SaveRoot.GameStatus == EnumGameStatus.MinerRolling)
        {
            return true;
        }
        return false; //for now.
    }
    [Command(EnumCommandCategory.Game)]
    public async Task PassAsync()
    {
        await mainGame.PassAsync();
    }
    public bool CanPlayCard => mainGame.SaveRoot.PlayedFaulty == false
        && mainGame.SaveRoot.GameStatus != EnumGameStatus.Safe //if a safe card was played, has to protect a color.  after choosing card to protect, status has to be resolve fire again.
        && mainGame.SaveRoot.GameStatus != EnumGameStatus.StartGambling
        && mainGame.SaveRoot.GameStatus != EnumGameStatus.FinishGambling
        && VMData.PlayerHand1.HandList.Any(x => x.CardCategory != EnumCardCategory.Gem);
    [Command(EnumCommandCategory.Game)]
    public async Task PlayCardAsync()
    {
        //let you play  but if invalid, then toast.  that is best for this game.
        if (VMData.PlayerHand1.HasSelectedObject() == false)
        {
            Toast.ShowUserErrorToast("You need to select a card to play");
            return;
        }
        int deck = VMData.PlayerHand1.ObjectSelected();
        YaBlewItCardInformation card = gameContainer.DeckList.GetSpecificItem(deck);
        if (card.CardCategory == EnumCardCategory.Gem)
        {
            Toast.ShowUserErrorToast("You can never play a gem card");
            return;
        }
        if (card.CardCategory == EnumCardCategory.None)
        {
            Toast.ShowUserErrorToast("The card category should never be none");
            return;
        }
        if (card.CardCategory == EnumCardCategory.Faulty)
        {
            if (mainGame.SaveRoot.GameStatus != EnumGameStatus.EndingTurn)
            {
                Toast.ShowUserErrorToast("The faulty card should be played at the end of your turn to reroll");
                return;
            }
            await mainGame.PlayCardAsync(deck);
            return;
        }
        if (card.CardCategory == EnumCardCategory.Safe)
        {
            if (mainGame.SaveRoot.GameStatus != EnumGameStatus.ResolveFire)
            {
                Toast.ShowUserErrorToast("The safe card can only be played when resolving fires");
                return;
            }
            if (SafeColorListClass.GetColorChoices().Count == 0)
            {
                Toast.ShowUserErrorToast("There are no more colors you need to protect.  Therefore, no need to play a safe card");
                return;
            }
            //you can protect more than one via fire.
            await mainGame.PlayCardAsync(deck);
            return;
        }
        if (card.CardCategory == EnumCardCategory.Jumper)
        {
            if (mainGame.SaveRoot.GameStatus != EnumGameStatus.ProspectorStarts)
            {
                Toast.ShowUserErrorToast("The jumper can only be played when prospector starts");
                return;
            }
            await mainGame.PlayCardAsync(deck);
            return;
        }
        if (card.CardCategory == EnumCardCategory.Fire)
        {
            Toast.ShowUserErrorToast("The fire card should never be in your hand");
            return;
        }
        Toast.ShowUserErrorToast("Not sure what to do about this card");
        await Task.CompletedTask;
    }
    public bool ShowSafeScreen => mainGame.SaveRoot.GameStatus == EnumGameStatus.Safe;
    private async Task ColorPicker_ItemClickedAsync(EnumColors piece)
    {
        await colorProcesses.ColorChosenAsync(piece);
    }
    public bool CanGamble => mainGame.SaveRoot.GameStatus == EnumGameStatus.GamblingDecision && mainGame.SaveRoot.PlayedFaulty == false;
    [Command(EnumCommandCategory.Game)]
    public async Task GambleAsync()
    {
        await mainGame.GambleAsync();
    }
    public bool CanTakeClaim => mainGame.SaveRoot.GameStatus == EnumGameStatus.GamblingDecision || mainGame.SaveRoot.GameStatus == EnumGameStatus.ProspectorContinues; //if you can gamble then you can take claim period.
    [Command(EnumCommandCategory.Game)]
    public async Task TakeClaimAsync()
    {
        await mainGame.TakeClaimAsync(false);
    }
    protected override bool CanEnableDeck()
    {
        return mainGame.SaveRoot.GameStatus == EnumGameStatus.StartGambling && mainGame.SaveRoot.DrewExtra == false;
    }
    protected override bool CanEnablePile1()
    {
        return false; //you do nothing with the discard piles.
    }
    protected override async Task ProcessDiscardClickedAsync()
    {
        //if we have anything, will be here.
        await Task.CompletedTask;
    }
    public override bool CanEnableAlways()
    {
        return true;
    }
}