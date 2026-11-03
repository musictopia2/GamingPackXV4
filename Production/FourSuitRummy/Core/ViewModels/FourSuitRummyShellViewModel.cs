namespace FourSuitRummy.Core.ViewModels;
public class FourSuitRummyShellViewModel(IGamePackageResolver mainContainer, CommandContainer container, IGameInfo gameData, BasicData basicData, IMultiplayerSaveState save, TestOptions test, IEventAggregator aggregator, IToast toast) : BasicMultiplayerShellViewModel<FourSuitRummyPlayerItem>(mainContainer, container, gameData, basicData, save, test, aggregator, toast)
{
}