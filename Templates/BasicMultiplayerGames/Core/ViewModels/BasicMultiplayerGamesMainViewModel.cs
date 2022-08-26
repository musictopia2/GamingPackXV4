
namespace BasicMultiplayerGames.Core.ViewModels;
[InstanceGame]
public class BasicMultiplayerGamesMainViewModel : BasicMultiplayerMainVM
{
    private readonly BasicMultiplayerGamesMainGameClass _mainGame; //if we don't need, delete.
    public BasicMultiplayerGamesVMData VMData { get; set; }
    public BasicMultiplayerGamesMainViewModel(CommandContainer commandContainer,
        BasicMultiplayerGamesMainGameClass mainGame,
        BasicData basicData,
        TestOptions test,
        IGamePackageResolver resolver,
        IEventAggregator aggregator,
        BasicMultiplayerGamesVMData data
        )
        : base(commandContainer, mainGame, basicData, test, resolver, aggregator)
    {
        _mainGame = mainGame;
        VMData = data;
    }
    //anything else needed is here.

}