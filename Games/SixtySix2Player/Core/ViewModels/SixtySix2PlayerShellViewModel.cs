namespace SixtySix2Player.Core.ViewModels;
public class SixtySix2PlayerShellViewModel(IGamePackageResolver mainContainer, CommandContainer container, IGameInfo gameData, BasicData basicData, IMultiplayerSaveState save, TestOptions test, IEventAggregator aggregator, IToast toast) : BasicTrickShellViewModel<SixtySix2PlayerPlayerItem>(mainContainer, container, gameData, basicData, save, test, aggregator, toast)
{
}