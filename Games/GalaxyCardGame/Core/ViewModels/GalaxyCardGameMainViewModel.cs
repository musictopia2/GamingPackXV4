namespace GalaxyCardGame.Core.ViewModels;
[InstanceGame]
public partial class GalaxyCardGameMainViewModel : BasicCardGamesVM<GalaxyCardGameCardInformation>
{
    private readonly GalaxyCardGameMainGameClass _mainGame; //if we don't need, delete.
    private readonly GalaxyCardGameVMData _model;
    private readonly IToast _toast;
    public GalaxyCardGameMainViewModel(CommandContainer commandContainer,
        GalaxyCardGameMainGameClass mainGame,
        GalaxyCardGameVMData viewModel,
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
        _mainGame.PlayerList.ForEach(player =>
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

            commandContainer.ExecutingChanged = CommandContainer_ExecutingChanged;

        });
        _model.TrickArea1.SendEnableProcesses(this, () => _mainGame.CanEnableTrickAreas);
        CreateCommands(commandContainer);
    }
    partial void CreateCommands(CommandContainer command);
    private void CommandContainer_ExecutingChanged()
    {
        if (_mainGame!.SaveRoot!.GameStatus == EnumGameStatus.PlaceSets)
        {
            _model.PlayerHand1!.AutoSelect = EnumHandAutoType.SelectAsMany;
        }
        else
        {
            _model.PlayerHand1!.AutoSelect = EnumHandAutoType.SelectOneOnly;
        }
    }
    public override bool CanEndTurn()
    {
        return _mainGame!.CanEndTurn();
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
    public bool CanMoon => _mainGame!.SingleInfo!.CanEnableMoon();
    [Command(EnumCommandCategory.Game)]
    public async Task MoonAsync()
    {
        var thisList = _model.PlayerHand1.ListSelectedObjects();
        if (_mainGame!.HasValidMoon(thisList) == false)
        {
            _toast.ShowUserErrorToast("Invalid Moon");
            return;
        }
        if (_mainGame.BasicData!.MultiPlayer)
        {
            var tempList = thisList.GetDeckListFromObjectList();
            await _mainGame.Network!.SendAllAsync("newmoon", tempList);
        }
        await _mainGame.PlayNewMoonAsync(thisList);
    }
}