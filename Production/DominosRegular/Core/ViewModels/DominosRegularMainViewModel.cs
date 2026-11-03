namespace DominosRegular.Core.ViewModels;
[InstanceGame]
public class DominosRegularMainViewModel(CommandContainer commandContainer,
            DominosRegularMainGameClass mainGame,
            DominosRegularVMData viewModel,
            BasicData basicData,
            IGamePackageResolver resolver,
            IEventAggregator aggregator) : DominoGamesVM<SimpleDominoInfo>(commandContainer, mainGame, viewModel, basicData, resolver, aggregator)
{
    public DominosRegularVMData VMData { get; set; } = viewModel;
    
    public HandObservable<SimpleDominoInfo> PlayerHand => VMData.PlayerHand1;
    public DominosBoneYardClass<SimpleDominoInfo> BoneYard => VMData.BoneYard;
    public PlayerCollection<DominosRegularPlayerItem> GetPlayerList => mainGame.SaveRoot.PlayerList;
    protected override bool CanEnableBoneYard()
    {
        return true;
    }
    public override bool CanEndTurn()
    {
        if (VMData.BoneYard!.HasBone())
        {
            return VMData.BoneYard.HasDrawn();
        }
        return true;
    }
    public GameBoardCP GetBoard => VMData.GameBoard1;
}