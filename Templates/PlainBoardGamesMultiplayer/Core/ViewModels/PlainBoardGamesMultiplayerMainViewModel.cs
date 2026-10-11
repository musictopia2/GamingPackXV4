namespace PlainBoardGamesMultiplayer.Core.ViewModels;
[InstanceGame]
public class PlainBoardGamesMultiplayerMainViewModel(CommandContainer commandContainer,
    PlainBoardGamesMultiplayerMainGameClass mainGame,
    BasicData basicData,
    IGamePackageResolver resolver,
    IEventAggregator aggregator,
    PlainBoardGamesMultiplayerVMData data
        ) : BasicMultiplayerMainVM(commandContainer, mainGame, basicData, resolver, aggregator)
{
    private readonly PlainBoardGamesMultiplayerMainGameClass _mainGame = mainGame; //if we don't need, delete.
    public PlainBoardGamesMultiplayerVMData VMData { get; set; } = data;
    //anything else needed is here.

}