namespace PlainBoardGamesMultiplayer.Core.ViewModels;
[InstanceGame]
public class PlainBoardGamesMultiplayerMainViewModel : BasicMultiplayerMainVM
{
    private readonly PlainBoardGamesMultiplayerMainGameClass _mainGame; //if we don't need, delete.
    public PlainBoardGamesMultiplayerVMData VMData { get; set; }
    public PlainBoardGamesMultiplayerMainViewModel(CommandContainer commandContainer,
        PlainBoardGamesMultiplayerMainGameClass mainGame,
        BasicData basicData,
        TestOptions test,
        IGamePackageResolver resolver,
        IEventAggregator aggregator,
        PlainBoardGamesMultiplayerVMData data
        )
        : base(commandContainer, mainGame, basicData, test, resolver, aggregator)
    {
        _mainGame = mainGame;
        VMData = data;
    }
    //anything else needed is here.

}