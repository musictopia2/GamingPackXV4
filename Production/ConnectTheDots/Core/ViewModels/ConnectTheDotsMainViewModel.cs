namespace ConnectTheDots.Core.ViewModels;
[InstanceGame]
public class ConnectTheDotsMainViewModel(CommandContainer commandContainer,
        ConnectTheDotsMainGameClass mainGame,
        BasicData basicData,
        IGamePackageResolver resolver,
        IEventAggregator aggregator,
        ConnectTheDotsVMData data) : BasicMultiplayerMainVM(commandContainer, mainGame, basicData, resolver, aggregator)
{
    public ConnectTheDotsVMData VMData { get; set; } = data;
}