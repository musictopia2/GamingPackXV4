namespace Millebournes.Core.ViewModels;
public class MillebournesShellViewModel(IGamePackageResolver mainContainer, CommandContainer container, IGameInfo gameData, BasicData basicData, IMultiplayerSaveState save, TestOptions test, IEventAggregator aggregator, IToast toast) : BasicMultiplayerShellViewModel<MillebournesPlayerItem>(mainContainer, container, gameData, basicData, save, test, aggregator, toast)
{
}