namespace Cribbage.Core.ViewModels;
public class CribbageShellViewModel(IGamePackageResolver mainContainer, CommandContainer container, IGameInfo gameData, BasicData basicData, IMultiplayerSaveState save, TestOptions test, IEventAggregator aggregator, IToast toast) : BasicMultiplayerShellViewModel<CribbagePlayerItem>(mainContainer, container, gameData, basicData, save, test, aggregator, toast)
{
}