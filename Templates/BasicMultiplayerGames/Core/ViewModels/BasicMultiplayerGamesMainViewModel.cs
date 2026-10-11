namespace BasicMultiplayerGames.Core.ViewModels;
[InstanceGame]
public class BasicMultiplayerGamesMainViewModel(CommandContainer commandContainer,
        BasicMultiplayerGamesMainGameClass mainGame,
        BasicData basicData,
        IGamePackageResolver resolver,
        IEventAggregator aggregator,
        BasicMultiplayerGamesVMData data
        ) : BasicMultiplayerMainVM(commandContainer, mainGame, basicData, resolver, aggregator)
{
    public BasicMultiplayerGamesVMData VMData { get; set; } = data;
    protected override Task CloseAsync()
    {
        return Task.CompletedTask;
    }

    protected override Task OpenAsync()
    {
        return Task.CompletedTask;
    }
}