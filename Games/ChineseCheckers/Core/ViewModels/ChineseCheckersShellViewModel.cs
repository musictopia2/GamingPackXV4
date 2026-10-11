namespace ChineseCheckers.Core.ViewModels;
public class ChineseCheckersShellViewModel(IGamePackageResolver mainContainer, CommandContainer container, IGameInfo gameData, BasicData basicData, IMultiplayerSaveState save, TestOptions test, IEventAggregator aggregator, IToast toast) : BeginningColorShellViewModel<ChineseCheckersPlayerItem>(mainContainer, container, gameData, basicData, save, test, aggregator, toast)
{
}