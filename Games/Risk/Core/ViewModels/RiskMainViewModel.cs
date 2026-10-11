namespace Risk.Core.ViewModels;
[InstanceGame]
public partial class RiskMainViewModel(CommandContainer commandContainer,
        RiskMainGameClass mainGame,
        BasicData basicData,
        IGamePackageResolver resolver,
        GameBoardProcesses gameBoardProcesses,
        RiskVMData data,
        AttackLogic attackLogic,
        IEventAggregator aggregator,
        IToast toast) : BasicMultiplayerMainVM(commandContainer, mainGame, basicData, resolver, aggregator)
{
    public RiskVMData VMData { get; set; } = data;
    protected override void AdditionalInitialize()
    {
        CreateCommands(CommandContainer);
        base.AdditionalInitialize();
    }
    partial void CreateCommands(CommandContainer command);
    public override bool CanEndTurn()
    {
        return mainGame.SaveRoot.Stage == EnumStageList.EndTurn;
    }
    public bool CanToNextStep
    {
        get
        {
            if (mainGame.SaveRoot.Stage == EnumStageList.Begin)
            {
                return VMData.PlayerHand1.HandList.Count <= 4;
            }
            return mainGame.SaveRoot.Stage == EnumStageList.StartAttack || mainGame.SaveRoot.Stage == EnumStageList.Move;
        }
    }
    [Command(EnumCommandCategory.Game)]
    public async Task ToNextStepAsync()
    {
        if (mainGame.BasicData.MultiPlayer)
        {
            await mainGame.Network!.SendAllAsync(nameof(IMultiplayerModel.ToNextStep));
        }
        await mainGame.ToNextStepAsync();
    }
    [Command(EnumCommandCategory.Game)]
    public async Task RollAttackAsync()
    {
        await attackLogic.RollAttackAsync();
    }

    public bool CanStartAttack => mainGame.SaveRoot.PreviousTerritory > 0 && mainGame.SaveRoot.CurrentTerritory > 0 && mainGame.SaveRoot.Stage == EnumStageList.StartAttack;
    [Command(EnumCommandCategory.Game)]
    public async Task StartAttackAsync()
    {
        await mainGame.StartAttackAsync();
    }
    public async Task TerritorySelectedAsync(TerritoryModel territory)
    {
        await gameBoardProcesses.TerritorySelectedAsync(territory, true);
    }
    public bool CanMoveArmies
    {
        get
        {
            if (mainGame.SaveRoot.Stage == EnumStageList.Move)
            {
                return VMData.ArmiesChosen > 0;
            }
            return VMData.ArmiesChosen >= 0;
        }
    }
    [Command(EnumCommandCategory.Game)]
    public async Task MoveArmiesAsync()
    {
        if (mainGame.BasicData.MultiPlayer)
        {
            await mainGame.Network!.SendAllAsync(nameof(IMultiplayerModel.MoveArmies), VMData.ArmiesChosen);
        }
        await gameBoardProcesses.MoveArmiesAsync(false);
    }
    [Command(EnumCommandCategory.Game)]
    public async Task PlaceArmiesAsync()
    {
        if (VMData.ArmiesChosen == 0)
        {
            toast.ShowUserErrorToast("Must choose at least one army to place");
            return;
        }
        if (mainGame.BasicData.MultiPlayer)
        {
            await mainGame.Network!.SendAllAsync(nameof(IMultiplayerModel.PlaceArmies), VMData.ArmiesChosen); //hopefully okay.
        }
        await gameBoardProcesses.PlaceArmiesAsync(false);
    }
    public bool GetCanReturnRiskCards() //did this way to eliminate the warnings.
    {
        if (mainGame is not null)
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
            toast.ShowUserErrorToast("Unable to return risk cards");
            return;
        }
        var cards = list.GetDeckListFromObjectList();
        if (cards.Count is not 3)
        {
            throw new CustomBasicException("Should not have been allowed to return risk cards");
        }
        if (mainGame.BasicData.MultiPlayer)
        {
            await mainGame.Network!.SendAllAsync(nameof(IMultiplayerModel.ReturnRiskCards), cards);
        }
        await mainGame.ReturnRiskCardsAsync(cards);
    }
}