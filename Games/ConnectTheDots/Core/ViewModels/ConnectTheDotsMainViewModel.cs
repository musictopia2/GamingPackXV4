namespace ConnectTheDots.Core.ViewModels;
[InstanceGame]
public class ConnectTheDotsMainViewModel : BasicMultiplayerMainVM
{
    public ConnectTheDotsVMData VMData { get; set; }
    public ConnectTheDotsMainViewModel(CommandContainer commandContainer,
        ConnectTheDotsMainGameClass mainGame,
        BasicData basicData,
        TestOptions test,
        IGamePackageResolver resolver,
        IEventAggregator aggregator,
        ConnectTheDotsVMData data
        )
        : base(commandContainer, mainGame, basicData, test, resolver, aggregator)
    {
        VMData = data;
    }
}