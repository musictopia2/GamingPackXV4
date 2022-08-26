namespace DominoBonesMultiplayerGames.Core.ViewModels;
[InstanceGame]
public class DominoBonesMultiplayerGamesMainViewModel : DominoGamesVM<SimpleDominoInfo>
{
    private readonly DominoBonesMultiplayerGamesMainGameClass _mainGame; //if we don't need, delete.
    public DominoBonesMultiplayerGamesVMData VMData { get; set; }
    public DominoBonesMultiplayerGamesMainViewModel(CommandContainer commandContainer,
            DominoBonesMultiplayerGamesMainGameClass mainGame,
            DominoBonesMultiplayerGamesVMData viewModel,
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
    public PlayerCollection<DominoBonesMultiplayerGamesPlayerItem> GetPlayerList => _mainGame.SaveRoot.PlayerList;
    protected override bool CanEnableBoneYard()
    {
        return true;
    }
    //anything else needed is here.

}