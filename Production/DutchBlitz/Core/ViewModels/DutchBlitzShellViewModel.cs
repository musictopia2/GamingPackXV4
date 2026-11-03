namespace DutchBlitz.Core.ViewModels;
public class DutchBlitzShellViewModel(IGamePackageResolver mainContainer, CommandContainer container, IGameInfo gameData, BasicData basicData, IMultiplayerSaveState save, TestOptions test, IEventAggregator aggregator, IToast toast) : BasicMultiplayerShellViewModel<DutchBlitzPlayerItem>(mainContainer, container, gameData, basicData, save, test, aggregator, toast)
{
}