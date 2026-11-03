namespace SequenceDice.Core.ViewModels;
public class SequenceDiceShellViewModel(IGamePackageResolver mainContainer, CommandContainer container, IGameInfo gameData, BasicData basicData, IMultiplayerSaveState save, TestOptions test, IEventAggregator aggregator, IToast toast) : BeginningColorShellViewModel<SequenceDicePlayerItem>(mainContainer, container, gameData, basicData, save, test, aggregator, toast)
{
}