namespace HitTheDeck.Core.ViewModels;
public class HitTheDeckShellViewModel(IGamePackageResolver mainContainer, CommandContainer container, IGameInfo gameData, BasicData basicData, IMultiplayerSaveState save, TestOptions test, IEventAggregator aggregator, IToast toast) : BasicMultiplayerShellViewModel<HitTheDeckPlayerItem>(mainContainer, container, gameData, basicData, save, test, aggregator, toast)
{
}