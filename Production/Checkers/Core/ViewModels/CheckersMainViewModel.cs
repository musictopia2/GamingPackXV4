namespace Checkers.Core.ViewModels;
[InstanceGame]
public partial class CheckersMainViewModel(CommandContainer commandContainer,
        CheckersMainGameClass mainGame,
        BasicData basicData,
        IGamePackageResolver resolver,
        IEventAggregator aggregator,
        CheckersVMData data) : BasicMultiplayerMainVM(commandContainer, mainGame, basicData, resolver, aggregator)
{
    public CheckersVMData VMData { get; set; } = data;
    protected override void AdditionalInitialize()
    {
        CreateCommands(CommandContainer);
        base.AdditionalInitialize();
    }
    partial void CreateCommands(CommandContainer command);
    public override bool CanEndTurn()
    {
        return mainGame.SaveRoot.GameStatus == EnumGameStatus.PossibleTie;
    }
    public bool CanTie
    {
        get
        {
            if (mainGame.SaveRoot.SpaceHighlighted > 0)
            {
                return false;
            }
            return mainGame.SaveRoot.ForcedToMove == false;
        }
    }
    [Command(EnumCommandCategory.Game)]
    public async Task TieAsync()
    {
        if (BasicData.MultiPlayer)
        {
            await mainGame.Network!.SendAllAsync("possibletie");
        }
        CommandContainer.ManuelFinish = true;
        await mainGame.ProcessTieAsync();
    }
}