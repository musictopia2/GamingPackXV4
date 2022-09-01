namespace Fluxx.Core.ViewModels;
[InstanceGame]
public partial class KeeperShowViewModel : BasicKeeperScreen
{
    private readonly IFluxxEvent _fluxxEvent;
    public KeeperShowViewModel(FluxxGameContainer gameContainer, KeeperContainer keeperContainer, IFluxxEvent fluxxEvent, IEventAggregator aggregator) : base(gameContainer, keeperContainer, aggregator)
    {
        _fluxxEvent = fluxxEvent;
        CreateCommands(CommandContainer);
    }
    partial void CreateCommands(CommandContainer command);
    [Command(EnumCommandCategory.Plain)]
    public async Task CloseKeeperAsync()
    {
        await _fluxxEvent.CloseKeeperScreenAsync();
    }
}