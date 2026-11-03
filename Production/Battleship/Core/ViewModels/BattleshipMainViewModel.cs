namespace Battleship.Core.ViewModels;
[InstanceGame]
[UseStateChanged]
public partial class BattleshipMainViewModel(CommandContainer commandContainer,
        BattleshipMainGameClass mainGame,
        BattleshipVMData viewModel,
        BasicData basicData,
        IGamePackageResolver resolver,
        IEventAggregator aggregator,
        IToast toast) : BasicMultiplayerMainVM(commandContainer, mainGame, basicData, resolver, aggregator)
{
    public BattleshipVMData VMData = viewModel;
    private BattleshipComputerAI? _ai = null;
    protected override void AdditionalInitialize()
    {
        if (BasicData.MultiPlayer == false)
        {
            _ai = Resolver.Resolve<BattleshipComputerAI>();
        }
        CreateCommands(CommandContainer);
        base.AdditionalInitialize();
    }
    partial void CreateCommands(CommandContainer command);
    public static int GamesPlayed { get; set; }
    [StateProperty]
    public partial bool ShipsHorizontal { get; set; } = true;
    private bool CanEnableShipOptions()
    {
        if (VMData.ShipDirectionsVisible == false)
        {
            return false;
        }
        return mainGame!.StatusOfGame == EnumStatusList.PlacingShips;
    }
    public bool CanShipDirection() => CanEnableShipOptions();
    [Command(EnumCommandCategory.Game)]
    public void ShipDirection(bool horizontal)
    {
        ShipsHorizontal = horizontal;
    }
    public bool CanMakeMove(Vector space)
    {
        if (mainGame.StatusOfGame == EnumStatusList.None)
        {
            toast.ShowUserErrorToast("Unable to make move because status is none");
            return false;
        }
        if (mainGame.StatusOfGame == EnumStatusList.PlacingShips)
        {
            if (mainGame.Ships.HasSelectedShip() == false)
            {
                toast.ShowUserErrorToast("Did not choose ship");
                return false;
            }
            return mainGame.Ships.CanPlaceShip(space, ShipsHorizontal);
        }
        return mainGame.GameBoard1.CanChooseSpace(space);
    }
    [Command(EnumCommandCategory.Game)]
    public async Task MakeMoveAsync(Vector space)
    {
        if (mainGame!.StatusOfGame == EnumStatusList.PlacingShips)
        {
            mainGame.Ships!.PlaceShip(space, ShipsHorizontal);
            if (mainGame.Ships.IsFinished() == false)
            {
                return;
            }
            await mainGame.HumanFinishedPlacingShipsAsync();
            return;
        }
        mainGame.CurrentSpace = space;
        if (BasicData!.MultiPlayer == true)
        {
            await mainGame.Network!.SendMoveAsync(space);
            CommandContainer!.ManuelFinish = true; //i think this is what i have to do now.
            mainGame.Network!.IsEnabled = true; //because has to see what it is.
            return;
        }
        bool hits = _ai!.HasHit(space);
        if (hits == true)
        {
            await mainGame.MakeMoveAsync(space, EnumWhatHit.Hit);
        }
        else
        {
            await mainGame.MakeMoveAsync(space, EnumWhatHit.Miss);
        }
    }
    public bool CanChooseShip()
    {
        return CanEnableShipOptions();
    }
    [Command(EnumCommandCategory.Game)]
    public void ChooseShip(EnumShipList ship)
    {
        VMData.ShipSelected = ship;
        mainGame.GameBoard1.HumanWaiting();
    }
}