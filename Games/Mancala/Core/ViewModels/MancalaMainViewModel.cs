namespace Mancala.Core.ViewModels;
[InstanceGame]
public class MancalaMainViewModel : BasicMultiplayerMainVM
{
    public MancalaVMData VMData { get; set; }
    public MancalaMainViewModel(CommandContainer commandContainer,
        MancalaMainGameClass mainGame,
        BasicData basicData,
        TestOptions test,
        IGamePackageResolver resolver,
        IEventAggregator aggregator,
        MancalaVMData data
        )
        : base(commandContainer, mainGame, basicData, test, resolver, aggregator)
    {
        VMData = data;
    }
}