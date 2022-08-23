namespace BasicGameFrameworkLibrary.Core.ViewModels;

public abstract class BasicTrickShellViewModel<P> : BasicMultiplayerShellViewModel<P>
    where P : class, IPlayerItem, new()
{
    public BasicTrickShellViewModel(IGamePackageResolver mainContainer,
        CommandContainer container,
        IGameInfo gameData,
        BasicData basicData,
        IMultiplayerSaveState save,
        TestOptions test,
        IEventAggregator aggregator,
        IToast toast
        ) : base(mainContainer, container, gameData, basicData, save, test, aggregator, toast)
    {
    }
    protected override BasicList<Type> GetAdditionalObjectsToReset()
    {
        return new BasicList<Type>() { typeof(IAdvancedTrickProcesses) }; //hopefully this simple.
    }
}