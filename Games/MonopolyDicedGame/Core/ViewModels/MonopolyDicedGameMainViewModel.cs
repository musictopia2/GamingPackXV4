namespace MonopolyDicedGame.Core.ViewModels;
[InstanceGame]
public class MonopolyDicedGameMainViewModel : BasicMultiplayerMainVM
{
    private readonly MonopolyDicedGameMainGameClass _mainGame; //if we don't need, delete.
    public MonopolyDicedGameVMData VMData { get; set; }
    public MonopolyDicedGameMainViewModel(CommandContainer commandContainer,
        MonopolyDicedGameMainGameClass mainGame,
        BasicData basicData,
        TestOptions test,
        IGamePackageResolver resolver,
        IEventAggregator aggregator,
        MonopolyDicedGameVMData data
        )
        : base(commandContainer, mainGame, basicData, test, resolver, aggregator)
    {
        _mainGame = mainGame;
        VMData = data;
    }
    //anything else needed is here.

}