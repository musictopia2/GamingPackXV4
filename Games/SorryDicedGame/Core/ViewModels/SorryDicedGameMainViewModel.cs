namespace SorryDicedGame.Core.ViewModels;
[InstanceGame]
public class SorryDicedGameMainViewModel : BasicMultiplayerMainVM
{
    private readonly SorryDicedGameMainGameClass _mainGame; //if we don't need, delete.
    public SorryDicedGameVMData VMData { get; set; }
    public SorryDicedGameMainViewModel(CommandContainer commandContainer,
        SorryDicedGameMainGameClass mainGame,
        BasicData basicData,
        TestOptions test,
        IGamePackageResolver resolver,
        IEventAggregator aggregator,
        SorryDicedGameVMData data
        )
        : base(commandContainer, mainGame, basicData, test, resolver, aggregator)
    {
        _mainGame = mainGame;
        VMData = data;
    }
    //anything else needed is here.

}