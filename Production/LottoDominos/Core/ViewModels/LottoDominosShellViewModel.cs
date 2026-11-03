namespace LottoDominos.Core.ViewModels;
public class LottoDominosShellViewModel(IGamePackageResolver mainContainer, CommandContainer container, IGameInfo gameData, BasicData basicData, IMultiplayerSaveState save, TestOptions test, IEventAggregator aggregator, IToast toast) : BasicMultiplayerShellViewModel<LottoDominosPlayerItem>(mainContainer, container, gameData, basicData, save, test, aggregator, toast)
{
}