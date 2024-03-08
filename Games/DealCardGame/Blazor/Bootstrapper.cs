
namespace DealCardGame.Blazor;
public class Bootstrapper : MultiplayerBasicBootstrapper<DealCardGameShellViewModel>
{
    public Bootstrapper(IStartUp starts, EnumGamePackageMode mode) : base(starts, mode)
    {
    }

    protected override Task RegisterTestsAsync()
    {
        TestData!.CardsToPass = 12;
        GetDIContainer.RegisterSingleton<ITestCardSetUp<DealCardGameCardInformation, DealCardGamePlayerItem>, TestCards>();
        return base.RegisterTestsAsync();
    }
    protected override Task ConfigureAsync(IGamePackageRegister register)
    {
        Core.DIFinishProcesses.GlobalDIAutoRegisterClass.RegisterNonSavedClasses(GetDIContainer);
        Core.DIFinishProcesses.SpecializedRegistrationHelpers.RegisterCommonMultplayerClasses(GetDIContainer);
        Core.DIFinishProcesses.AutoResetClass.RegisterAutoResets();
        register.RegisterSingleton<IDeckCount, DealCardGameDeckCount>();
        //anything that needs to be registered will be here.
        return Task.CompletedTask;
    }

    //this part should not change
    protected override void FinishRegistrations(IGamePackageRegister register)
    {
        register.RegisterType<DealCardGameShellViewModel>(); //has to use interface part to make it work with source generators.
        Core.DIFinishProcesses.GlobalDIFinishClass.FinishDIRegistrations(GetDIContainer);
        Core.AutoResumeContexts.GlobalRegistrations.Register();
    }
}
public class TestCards : ITestCardSetUp<DealCardGameCardInformation, DealCardGamePlayerItem>
{
    Task ITestCardSetUp<DealCardGameCardInformation, DealCardGamePlayerItem>.SetUpTestHandsAsync(PlayerCollection<DealCardGamePlayerItem> playerList, IListShuffler<DealCardGameCardInformation> deckList)
    {
        var player = playerList.GetSelf();
        var card = deckList.First(x => x.ActionCategory == EnumActionCategory.Gos);
        player.StartUpList.Add(card);
        return Task.CompletedTask;
    }
}