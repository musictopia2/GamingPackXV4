namespace HuseHearts.Core.ViewModels;
public class HuseHeartsShellViewModel(IGamePackageResolver mainContainer, CommandContainer container, IGameInfo gameData, BasicData basicData, IMultiplayerSaveState save, TestOptions test, IEventAggregator aggregator, IToast toast) : BasicTrickShellViewModel<HuseHeartsPlayerItem>(mainContainer, container, gameData, basicData, save, test, aggregator, toast)
{
}