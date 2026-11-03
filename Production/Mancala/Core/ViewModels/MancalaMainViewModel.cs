namespace Mancala.Core.ViewModels;
[InstanceGame]
public class MancalaMainViewModel(CommandContainer commandContainer,
        MancalaMainGameClass mainGame,
        BasicData basicData,
        IGamePackageResolver resolver,
        IEventAggregator aggregator,
        MancalaVMData data) : BasicMultiplayerMainVM(commandContainer, mainGame, basicData, resolver, aggregator)
{
    public MancalaVMData VMData { get; set; } = data;
}