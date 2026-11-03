namespace GermanWhist.Core.ViewModels;
public class GermanWhistShellViewModel(IGamePackageResolver mainContainer, CommandContainer container, IGameInfo gameData, BasicData basicData, IMultiplayerSaveState save, TestOptions test, IEventAggregator aggregator, IToast toast) : BasicTrickShellViewModel<GermanWhistPlayerItem>(mainContainer, container, gameData, basicData, save, test, aggregator, toast)
{
}