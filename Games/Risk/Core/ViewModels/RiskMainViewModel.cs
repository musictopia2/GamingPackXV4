namespace Risk.Core.ViewModels;
[InstanceGame]
public partial class RiskMainViewModel : BasicMultiplayerMainVM
{
    private readonly CommandContainer _commandContainer;
    private readonly RiskMainGameClass _mainGame;
    private readonly GameBoardProcesses _gameBoardProcesses;
    private readonly AttackLogic _attackLogic;
    private readonly IToast _toast;
    public RiskVMData VMData { get; set; }
    public RiskMainViewModel(CommandContainer commandContainer,
        RiskMainGameClass mainGame,
        BasicData basicData,
        TestOptions test,
        IGamePackageResolver resolver,
        GameBoardProcesses gameBoardProcesses,
        AttackLogic attackLogic,
        IEventAggregator aggregator,
        IToast toast
        )
        : base(commandContainer, mainGame, basicData, test, resolver, aggregator)
    {
        _commandContainer = commandContainer;
        _mainGame = mainGame;
        VMData = aa1.Resolver!.Resolve<RiskVMData>();
        _gameBoardProcesses = gameBoardProcesses;
        _attackLogic = attackLogic;
        _toast = toast;
        CreateCommands(commandContainer);
    }
    partial void CreateCommands(CommandContainer command);
    public override bool CanEndTurn()
    {
        return _mainGame.SaveRoot.Stage == EnumStageList.EndTurn;
    }
    public bool CanToNextStep
    {
        get
        {
            if (_mainGame.SaveRoot.Stage == EnumStageList.Begin)
            {
                return VMData.PlayerHand1.HandList.Count <= 4;
            }
            return _mainGame.SaveRoot.Stage == EnumStageList.StartAttack || _mainGame.SaveRoot.Stage == EnumStageList.Move;
        }
    }
    [Command(EnumCommandCategory.Game)]
    public async Task ToNextStepAsync()
    {
        if (_mainGame.BasicData.MultiPlayer)
        {
            await _mainGame.Network!.SendAllAsync(nameof(IMultiplayerModel.ToNextStep));
        }
        await _mainGame.ToNextStepAsync();
    }
    [Command(EnumCommandCategory.Game)]
    public async Task RollAttackAsync()
    {
        await _attackLogic.RollAttackAsync();
    }

    public bool CanStartAttack => _mainGame.SaveRoot.PreviousTerritory > 0 && _mainGame.SaveRoot.CurrentTerritory > 0 && _mainGame.SaveRoot.Stage == EnumStageList.StartAttack;
    [Command(EnumCommandCategory.Game)]
    public async Task StartAttackAsync()
    {
        await _mainGame.StartAttackAsync();
    }
    public async Task TerritorySelectedAsync(TerritoryModel territory)
    {
        await _gameBoardProcesses.TerritorySelectedAsync(territory, true);
    }
    public bool CanMoveArmies
    {
        get
        {
            if (_mainGame.SaveRoot.Stage == EnumStageList.Move)
            {
                return VMData.ArmiesChosen > 0;
            }
            return VMData.ArmiesChosen >= 0;
        }
    }
    [Command(EnumCommandCategory.Game)]
    public async Task MoveArmiesAsync()
    {
        if (_mainGame.BasicData.MultiPlayer)
        {
            await _mainGame.Network!.SendAllAsync(nameof(IMultiplayerModel.MoveArmies), VMData.ArmiesChosen);
        }
        await _gameBoardProcesses.MoveArmiesAsync(false);
    }
    [Command(EnumCommandCategory.Game)]
    public async Task PlaceArmiesAsync()
    {
        if (VMData.ArmiesChosen == 0)
        {
            _toast.ShowUserErrorToast("Must choose at least one army to place");
            return;
        }
        if (_mainGame.BasicData.MultiPlayer)
        {
            await _mainGame.Network!.SendAllAsync(nameof(IMultiplayerModel.PlaceArmies), VMData.ArmiesChosen); //hopefully okay.
        }
        await _gameBoardProcesses.PlaceArmiesAsync(false);
    }
    public bool GetCanReturnRiskCards() //did this way to eliminate the warnings.
    {
        if (_mainGame is not null)
        {
            return true;
        }
        return true;
    }
    [Command(EnumCommandCategory.Game)]
    public async Task ReturnRiskCardsAsync()
    {
        var list = VMData.PlayerHand1.ListSelectedObjects();
        if (list.CanReturnRiskCards == false)
        {
            _toast.ShowUserErrorToast("Unable to return risk cards");
            return;
        }
        var cards = list.GetDeckListFromObjectList();
        if (cards.Count is not 3)
        {
            throw new CustomBasicException("Should not have been allowed to return risk cards");
        }
        if (_mainGame.BasicData.MultiPlayer)
        {
            await _mainGame.Network!.SendAllAsync(nameof(IMultiplayerModel.ReturnRiskCards), cards);
        }
        await _mainGame.ReturnRiskCardsAsync(cards);
    }
}