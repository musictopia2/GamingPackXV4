namespace ThreeLetterFun.Core.ViewModels;
public class ThreeLetterFunShellViewModel(IGamePackageResolver mainContainer, CommandContainer container, IGameInfo gameData, BasicData basicData, IMultiplayerSaveState save, TestOptions test, IEventAggregator aggregator, IToast toast)
    : BasicMultiplayerShellViewModel<ThreeLetterFunPlayerItem>(mainContainer, container, gameData, 
        basicData, save, test, aggregator, toast)
{
    protected override EnumOpeningCategory OpeningCategory => EnumOpeningCategory.GamePrompt;
}