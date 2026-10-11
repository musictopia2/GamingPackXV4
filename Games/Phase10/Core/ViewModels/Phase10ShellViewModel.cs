namespace Phase10.Core.ViewModels;
public class Phase10ShellViewModel(IGamePackageResolver mainContainer, CommandContainer container, IGameInfo gameData, BasicData basicData, IMultiplayerSaveState save, TestOptions test, IEventAggregator aggregator, IToast toast) : BasicMultiplayerShellViewModel<Phase10PlayerItem>(mainContainer, container, gameData, basicData, save, test, aggregator, toast)
{
}