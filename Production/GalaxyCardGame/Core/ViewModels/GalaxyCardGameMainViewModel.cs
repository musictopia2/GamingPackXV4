namespace GalaxyCardGame.Core.ViewModels;
[InstanceGame]
public partial class GalaxyCardGameMainViewModel(CommandContainer commandContainer,
        GalaxyCardGameMainGameClass mainGame,
        GalaxyCardGameVMData viewModel,
        BasicData basicData,
        IGamePackageResolver resolver,
        IEventAggregator aggregator,
        IToast toast) : BasicCardGamesVM<GalaxyCardGameCardInformation>(commandContainer, mainGame, viewModel, basicData, resolver, aggregator, toast)
{
    protected override void AdditionalInitialize()
    {
        viewModel.Deck1.NeverAutoDisable = true;
        mainGame.PlayerList.ForEach(player =>
        {
            if (player.PlanetHand == null)
            {
                throw new CustomBasicException("No planet hand.  Rethink");
            }
            if (player.Moons == null)
            {
                throw new CustomBasicException("No moons.  Rethink");
            }
            player.PlanetHand.SendEnableProcesses(this, () =>
            {
                if (player.PlayerCategory != EnumPlayerCategory.Self)
                {
                    return false;
                }
                if (mainGame.SaveRoot!.GameStatus != EnumGameStatus.PlaceSets)
                {
                    return false;
                }
                return mainGame.HasAutomaticPlanet() || player.PlanetHand.HandList.Count == 0;
            });
            player.Moons.SendEnableProcesses(this, (() =>
            {
                if (player.PlayerCategory != EnumPlayerCategory.Self)
                {
                    return false;
                }
                return player.CanEnableMoon();
            }));

            CommandContainer.ExecutingChanged = CommandContainer_ExecutingChanged;

        });
        viewModel.TrickArea1.SendEnableProcesses(this, () => mainGame.CanEnableTrickAreas);
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
        if (mainGame!.SaveRoot!.GameStatus == EnumGameStatus.PlaceSets)
        {
            viewModel.PlayerHand1!.AutoSelect = EnumHandAutoType.SelectAsMany;
        }
        else
        {
            viewModel.PlayerHand1!.AutoSelect = EnumHandAutoType.SelectOneOnly;
        }
    }
    public override bool CanEndTurn()
    {
        return mainGame!.CanEndTurn();
    }
    protected override bool CanEnableDeck()
    {
        return false;
    }
    protected override bool CanEnablePile1()
    {
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
    public bool CanMoon => mainGame!.SingleInfo!.CanEnableMoon();
    [Command(EnumCommandCategory.Game)]
    public async Task MoonAsync()
    {
        var thisList = viewModel.PlayerHand1.ListSelectedObjects();
        if (mainGame!.HasValidMoon(thisList) == false)
        {
            Toast.ShowUserErrorToast("Invalid Moon");
            return;
        }
        if (mainGame.BasicData!.MultiPlayer)
        {
            var tempList = thisList.GetDeckListFromObjectList();
            await mainGame.Network!.SendAllAsync("newmoon", tempList);
        }
        await mainGame.PlayNewMoonAsync(thisList);
    }
}