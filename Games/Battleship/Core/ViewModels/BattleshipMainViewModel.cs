namespace Battleship.Core.ViewModels;
[InstanceGame]
public partial class BattleshipMainViewModel : BasicMultiplayerMainVM
{
    private readonly BattleshipMainGameClass _mainGame; //if we don't need, delete.
    public BattleshipVMData VMData;
    private readonly BasicData _basicData;
    private readonly IToast _toast;
    private readonly BattleshipComputerAI? _ai;
    public BattleshipMainViewModel(CommandContainer commandContainer,
        BattleshipMainGameClass mainGame,
        BattleshipVMData viewModel,
        BasicData basicData,
        TestOptions test,
        IGamePackageResolver resolver,
        IEventAggregator aggregator,
        IToast toast
        )
        : base(commandContainer, mainGame, basicData, test, resolver, aggregator)
    {
        _mainGame = mainGame;
        VMData = viewModel;
        _basicData = basicData;
        _toast = toast;
        if (_basicData.MultiPlayer == false)
        {
            _ai = resolver.Resolve<BattleshipComputerAI>();
        }
        CreateCommands(commandContainer);
    }
    partial void CreateCommands(CommandContainer command);
    public static int GamesPlayed { get; set; }
    public bool ShipsHorizontal { get; set; } = true;
    private bool CanEnableShipOptions()
    {
        if (VMData.ShipDirectionsVisible == false)
        {
            return false;
        }
        return _mainGame!.StatusOfGame == EnumStatusList.PlacingShips;
    }
    public bool CanShipDirection() => CanEnableShipOptions();
    [Command(EnumCommandCategory.Game)]
    public void ShipDirection(bool horizontal)
    {
        ShipsHorizontal = horizontal;
    }
    public bool CanMakeMove(Vector space)
    {
        if (_mainGame.StatusOfGame == EnumStatusList.None)
        {
            _toast.ShowUserErrorToast("Unable to make move because status is none");
            return false;
        }
        if (_mainGame.StatusOfGame == EnumStatusList.PlacingShips)
        {
            if (_mainGame.Ships.HasSelectedShip() == false)
            {
                _toast.ShowUserErrorToast("Did not choose ship");
                return false;
            }
            return _mainGame.Ships.CanPlaceShip(space, ShipsHorizontal);
        }
        return _mainGame.GameBoard1.CanChooseSpace(space);
    }
    [Command(EnumCommandCategory.Game)]
    public async Task MakeMoveAsync(Vector space)
    {
        if (_mainGame!.StatusOfGame == EnumStatusList.PlacingShips)
        {
            _mainGame.Ships!.PlaceShip(space, ShipsHorizontal);
            if (_mainGame.Ships.IsFinished() == false)
            {
                return;
            }
            await _mainGame.HumanFinishedPlacingShipsAsync();
            return;
        }
        _mainGame.CurrentSpace = space;
        if (_basicData!.MultiPlayer == true)
        {
            await _mainGame.Network!.SendMoveAsync(space);
            CommandContainer!.ManuelFinish = true; //i think this is what i have to do now.
            _mainGame.Network!.IsEnabled = true; //because has to see what it is.
            return;
        }
        bool hits = _ai!.HasHit(space);
        if (hits == true)
        {
            await _mainGame.MakeMoveAsync(space, EnumWhatHit.Hit);
        }
        else
        {
            await _mainGame.MakeMoveAsync(space, EnumWhatHit.Miss);
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
        _mainGame.GameBoard1.HumanWaiting();
    }
}