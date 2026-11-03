namespace SorryDicedGame.Core.ViewModels;
public class SorryDicedGameShellViewModel(IGamePackageResolver mainContainer, CommandContainer container, IGameInfo gameData, BasicData basicData, IMultiplayerSaveState save, TestOptions test, IEventAggregator aggregator, IToast toast) : BeginningColorShellViewModel<SorryDicedGamePlayerItem>(mainContainer, container, gameData, basicData, save, test, aggregator, toast)
{
}