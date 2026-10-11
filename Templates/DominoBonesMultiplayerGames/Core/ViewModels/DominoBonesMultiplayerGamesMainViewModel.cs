namespace DominoBonesMultiplayerGames.Core.ViewModels;
[InstanceGame]
public class DominoBonesMultiplayerGamesMainViewModel(CommandContainer commandContainer,
        DominoBonesMultiplayerGamesMainGameClass mainGame,
        DominoBonesMultiplayerGamesVMData viewModel,
        BasicData basicData,
        IGamePackageResolver resolver,
        IEventAggregator aggregator
        ) : DominoGamesVM<SimpleDominoInfo>(commandContainer, mainGame, viewModel, basicData, resolver, aggregator)
{
    public DominoBonesMultiplayerGamesVMData VMData { get; set; } = viewModel;

    public HandObservable<SimpleDominoInfo> PlayerHand => VMData.PlayerHand1;
    public DominosBoneYardClass<SimpleDominoInfo> BoneYard => VMData.BoneYard;
    public PlayerCollection<DominoBonesMultiplayerGamesPlayerItem> GetPlayerList => mainGame.SaveRoot.PlayerList;
    protected override bool CanEnableBoneYard()
    {
        return true;
    }
    //anything else needed is here.

}