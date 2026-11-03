namespace ItalianDominos.Core.ViewModels;
public class ItalianDominosShellViewModel(IGamePackageResolver mainContainer, CommandContainer container, IGameInfo gameData, BasicData basicData, IMultiplayerSaveState save, TestOptions test, IEventAggregator aggregator, IToast toast) : BasicMultiplayerShellViewModel<ItalianDominosPlayerItem>(mainContainer, container, gameData, basicData, save, test, aggregator, toast)
{
}