namespace DominosRegular.Core.ViewModels;
[InstanceGame]
public class DominosRegularMainViewModel : DominoGamesVM<SimpleDominoInfo>
{
    private readonly DominosRegularMainGameClass _mainGame; //if we don't need, delete.
    public DominosRegularVMData VMData { get; set; }
    public DominosRegularMainViewModel(CommandContainer commandContainer,
            DominosRegularMainGameClass mainGame,
            DominosRegularVMData viewModel,
            BasicData basicData,
            TestOptions test,
            IGamePackageResolver resolver,
            IEventAggregator aggregator
        )
        : base(commandContainer, mainGame, viewModel, basicData, test, resolver, aggregator)
    {
        _mainGame = mainGame;
        VMData = viewModel;
    }
    public HandObservable<SimpleDominoInfo> PlayerHand => VMData.PlayerHand1;
    public DominosBoneYardClass<SimpleDominoInfo> BoneYard => VMData.BoneYard;
    public PlayerCollection<DominosRegularPlayerItem> GetPlayerList => _mainGame.SaveRoot.PlayerList;
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