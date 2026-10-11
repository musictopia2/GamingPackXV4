namespace TeeItUp.Core.ViewModels;
public class TeeItUpShellViewModel(IGamePackageResolver mainContainer, CommandContainer container, IGameInfo gameData, BasicData basicData, IMultiplayerSaveState save, TestOptions test, IEventAggregator aggregator, IToast toast) : BasicMultiplayerShellViewModel<TeeItUpPlayerItem>(mainContainer, container, gameData, basicData, save, test, aggregator, toast)
{
}