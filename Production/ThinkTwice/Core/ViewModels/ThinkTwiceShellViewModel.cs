namespace ThinkTwice.Core.ViewModels;
public class ThinkTwiceShellViewModel(IGamePackageResolver mainContainer, CommandContainer container, IGameInfo gameData, BasicData basicData, IMultiplayerSaveState save, TestOptions test, IEventAggregator aggregator, IToast toast) : BasicMultiplayerShellViewModel<ThinkTwicePlayerItem>(mainContainer, container, gameData, basicData, save, test, aggregator, toast)
{
}