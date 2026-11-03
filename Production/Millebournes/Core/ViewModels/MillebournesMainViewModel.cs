namespace Millebournes.Core.ViewModels;
[InstanceGame]
[UseStateChanged]
public partial class MillebournesMainViewModel(CommandContainer commandContainer,
        MillebournesMainGameClass mainGame,
        MillebournesVMData viewModel,
        BasicData basicData,
        IGamePackageResolver resolver,
        MillebournesGameContainer gameContainer,
        IEventAggregator aggregator,
        IToast toast) : BasicCardGamesVM<MillebournesCardInformation>(commandContainer, mainGame, viewModel, basicData, resolver, aggregator, toast)
{
    protected override void AdditionalInitialize()
    {
        viewModel.Deck1.NeverAutoDisable = false;
        viewModel.Pile2!.PileClickedAsync = Pile2_PileClickedAsync;
        CommandContainer!.ExecutingChanged = CommandContainer_ExecutingChanged;
        viewModel.Pile1.Text = "New Card";
        viewModel.Pile1.Visible = false;
        viewModel.Pile2.Text = "Throw Away";
        viewModel.Pile2.FirstLoad(new MillebournesCardInformation());
        viewModel.PlayerHand1.AutoSelect = EnumHandAutoType.None;
        viewModel.PlayerHand1.Maximum = 6;
        gameContainer.TeamClickAsync = ProcessTeamClickAsync;
        gameContainer.LoadCoupe = LoadCoupe;
        gameContainer.CloseCoupe = CloseCoupe;
        CreateCommands(CommandContainer);
        base.AdditionalInitialize();
    }
    partial void CreateCommands(CommandContainer command);
    [StateProperty]
    public partial CoupeViewModel? CoupeScreen { get; set; }
    private void LoadCoupe()
    {
        if (CoupeScreen != null)
        {
            throw new CustomBasicException("Coupe screen was already loaded.  Rethink");
        }
        CoupeScreen = Resolver.Resolve<CoupeViewModel>();
    }
    private void CloseCoupe()
    {
        if (CoupeScreen == null)
        {
            return;
        }
        CoupeScreen.Close();
        CoupeScreen = null;
    }
    private void CommandContainer_ExecutingChanged()
    {
        viewModel.Pile1.ReportCanExecuteChange();
        gameContainer!.TeamList.ForEach(thisTeam =>
        {
            thisTeam.EnableChange();
        });
    }
    private async Task Pile2_PileClickedAsync()
    {
        int deck = mainGame!.FindDeck;
        if (deck == 0)
        {
            Toast.ShowUserErrorToast("Sorry, needs to select a card to throw away");
            return;
        }
        await mainGame!.ThrowawayCardAsync(deck);
    }
    protected override Task CloseAsync()
    {
        CloseCoupe();
        return base.CloseAsync();
    }
    protected override bool CanEnableDeck()
    {
        return false;
    }
    protected override bool CanEnablePile1()
    {
        return viewModel.Pile1!.Visible;
    }
    protected override Task ProcessDiscardClickedAsync()
    {
        if (viewModel.Pile1!.PileEmpty())
        {
            throw new CustomBasicException("Since there is no current card, should have never been enabled");
        }
        viewModel.PlayerHand1!.UnselectAllObjects();
        if (viewModel.Pile1.CardSelected() == 0)
        {
            viewModel.Pile1.IsSelected(true);
        }
        else
        {
            viewModel.Pile1.IsSelected(false);
        }
        CommandContainer!.ManuelFinish = false;
        CommandContainer.IsExecuting = false;
        return Task.CompletedTask;
    }
    public override bool CanEnableAlways()
    {
        return true;
    }
    protected override async Task ProcessHandClickedAsync(MillebournesCardInformation thisCard, int index)
    {
        if (thisCard.IsSelected == true)
        {
            thisCard.IsSelected = false;
            return;
        }
        if (viewModel.Pile1!.CardSelected() > 0)
        {
            viewModel.Pile1.IsSelected(false);
            thisCard.IsSelected = true;
            return;
        }
        viewModel.PlayerHand1!.UnselectAllObjects();
        thisCard.IsSelected = true;
        await Task.CompletedTask;
    }
    private async Task ProcessTeamClickAsync(EnumPileType pileType, int team)
    {
        int newDeck = mainGame!.FindDeck;
        if (pileType == EnumPileType.None)
        {
            throw new CustomBasicException("Must have a pile that was clicked on");
        }
        if (team == 0)
        {
            throw new CustomBasicException("No team sent when clicking a pile");
        }
        if (newDeck == 0)
        {
            Toast.ShowUserErrorToast("Sorry, you must select a card first");
            return;
        }
        var thisTeam = gameContainer!.FindTeam(team);
        thisTeam.CurrentCard = gameContainer.DeckList!.GetSpecificItem(newDeck);
        string message;
        if (pileType == EnumPileType.Miles)
        {
            if (team != mainGame.SaveRoot!.CurrentTeam)
            {
                throw new CustomBasicException($"The miles should have been disabled since you cannot play miles for another team.  The team clicked was {team} .  However, the current team is {mainGame.SaveRoot.CurrentTeam}");
            }
            if (thisTeam.CanPlaceMiles(out message) == false)
            {
                Toast.ShowUserErrorToast(message);
                viewModel.PlayerHand1!.UnselectAllObjects();
                return;
            }
        }
        if (pileType == EnumPileType.Hazard)
        {
            if (mainGame.SaveRoot!.CurrentTeam == team)
            {
                if (thisTeam.CanFixHazard(out message) == false)
                {
                    Toast.ShowUserErrorToast(message);
                    viewModel.PlayerHand1!.UnselectAllObjects();
                    return;
                }
            }
            else if (mainGame.SaveRoot.CurrentTeam != team && thisTeam.CanGiveHazard(out message) == false)
            {
                Toast.ShowUserErrorToast(message);
                viewModel.PlayerHand1!.UnselectAllObjects();
                return;
            }
        }
        if (pileType == EnumPileType.Speed)
        {
            if (mainGame.SaveRoot!.CurrentTeam == team)
            {
                if (thisTeam.CanEndSpeedLimit(out message) == false)
                {
                    Toast.ShowUserErrorToast(message);
                    viewModel.PlayerHand1!.UnselectAllObjects();
                    return;
                }
            }
            else if (mainGame.SaveRoot.CurrentTeam != team && thisTeam.CanGiveSpeedLimit(out message) == false)
            {
                Toast.ShowUserErrorToast(message);
                viewModel.PlayerHand1!.UnselectAllObjects();
                return;
            }
        }
        if (pileType == EnumPileType.Safety)
        {
            if (mainGame.SaveRoot!.CurrentTeam != team)
            {
                throw new CustomBasicException("Cannot place a safety for another team.  Therefore, this should have been disabled");
            }
            if (thisTeam.CanPlaceSafety(out message) == false)
            {
                Toast.ShowUserErrorToast(message);
                viewModel.PlayerHand1!.UnselectAllObjects();
                return;
            }
        }
        if (mainGame.BasicData!.MultiPlayer == true)
        {
            SendPlay thisSend = new();
            thisSend.Deck = newDeck;
            thisSend.Pile = pileType;
            thisSend.Team = team;
            await mainGame.Network!.SendAllAsync("regularplay", thisSend);
        }
        await mainGame.PlayAsync(newDeck, pileType, team, false);
    }
    [Command(EnumCommandCategory.Plain)]
    public async Task SafetyClickAsync(TeamCP team)
    {
        if (gameContainer.TeamClickAsync == null)
        {
            throw new CustomBasicException("Nobody is handling the team click.  Rethink");
        }
        await gameContainer.TeamClickAsync.Invoke(EnumPileType.Safety, team.TeamNumber);
    }
}