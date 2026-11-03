namespace Risk.Core.ViewModels;
public class RiskShellViewModel(IGamePackageResolver mainContainer, CommandContainer container, IGameInfo gameData, BasicData basicData, IMultiplayerSaveState save, TestOptions test, IEventAggregator aggregator, IToast toast) : BeginningColorShellViewModel<RiskPlayerItem>(mainContainer, container, gameData, basicData, save, test, aggregator, toast)
{
}