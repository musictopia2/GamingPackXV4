namespace Concentration.Core.ViewModels;
public class ConcentrationShellViewModel(IGamePackageResolver mainContainer, CommandContainer container, IGameInfo gameData, BasicData basicData, IMultiplayerSaveState save, TestOptions test, IEventAggregator aggregator, IToast toast) : BasicMultiplayerShellViewModel<ConcentrationPlayerItem>(mainContainer, container, gameData, basicData, save, test, aggregator, toast)
{
}