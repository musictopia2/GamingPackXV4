namespace ShipCaptainCrew.Core.ViewModels;
public class ShipCaptainCrewShellViewModel(IGamePackageResolver mainContainer, CommandContainer container, IGameInfo gameData, BasicData basicData, IMultiplayerSaveState save, TestOptions test, IEventAggregator aggregator, IToast toast) : BasicMultiplayerShellViewModel<ShipCaptainCrewPlayerItem>(mainContainer, container, gameData, basicData, save, test, aggregator, toast)
{
}