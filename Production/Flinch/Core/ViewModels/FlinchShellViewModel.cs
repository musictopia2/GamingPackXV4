namespace Flinch.Core.ViewModels;
public class FlinchShellViewModel(IGamePackageResolver mainContainer, CommandContainer container, IGameInfo gameData, BasicData basicData, IMultiplayerSaveState save, TestOptions test, IEventAggregator aggregator, IToast toast) : BasicMultiplayerShellViewModel<FlinchPlayerItem>(mainContainer, container, gameData, basicData, save, test, aggregator, toast)
{
}