namespace LifeBoardGame.Core.ViewModels;
public class LifeBoardGameShellViewModel(IGamePackageResolver mainContainer, CommandContainer container, IGameInfo gameData, BasicData basicData, IMultiplayerSaveState save, TestOptions test, IEventAggregator aggregator, IToast toast) : BeginningColorShellViewModel<LifeBoardGamePlayerItem>(mainContainer, container, gameData, basicData, save, test, aggregator, toast)
{
    protected override bool CanOpenMainAfterColors => false;
    protected override async Task LoadStartingOptionsAsync()
    {
        LifeBoardGameSaveInfo saveRoot = Resolver!.Resolve<LifeBoardGameSaveInfo>();
        if (saveRoot.GameStatus == EnumWhatStatus.NeedChooseGender)
        {
            await OpenPromptAsync();
            return;
        }
        await base.LoadStartingOptionsAsync();
    }
    protected override async Task AfterClosingPromptAsync()
    {
        LifeBoardGameMainGameClass game = Resolver!.Resolve<LifeBoardGameMainGameClass>();
        game.SaveRoot.GameStatus = EnumWhatStatus.NeedChooseFirstOption; //belongs here.
        await StartNewGameAsync();
        await game.AfterChoosingGenderAsync();
    }
}