namespace PassOutDiceGame.Core.ViewModels;
public class PassOutDiceGameShellViewModel(IGamePackageResolver mainContainer, CommandContainer container, IGameInfo gameData, BasicData basicData, IMultiplayerSaveState save, TestOptions test, IEventAggregator aggregator, IToast toast) : BeginningColorShellViewModel<PassOutDiceGamePlayerItem>(mainContainer, container, gameData, basicData, save, test, aggregator, toast)
{
}