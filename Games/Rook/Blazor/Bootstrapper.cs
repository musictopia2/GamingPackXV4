namespace Rook.Blazor;
public class TestCards : ITestCardSetUp<RookCardInformation, RookPlayerItem>
{
    Task ITestCardSetUp<RookCardInformation, RookPlayerItem>.SetUpTestHandsAsync(PlayerCollection<RookPlayerItem> playerList, IListShuffler<RookCardInformation> deckList)
    {
        var self = playerList.GetSelf();
        var card = deckList.Single(x => x.IsBird == true);
        self.StartUpList.Add(card); //to guarantee i get a rook card so i can see how it looks
        return Task.CompletedTask;
    }
}
public class Bootstrapper : MultiplayerBasicBootstrapper<RookShellViewModel>
{
    public Bootstrapper(IStartUp starts, EnumGamePackageMode mode) : base(starts, mode)
    {
    }

    protected override Task RegisterTestsAsync()
    {
        GetDIContainer.RegisterSingleton<ITestCardSetUp<RookCardInformation, RookPlayerItem>, TestCards>();
        return Task.CompletedTask;
    }
    protected override Task ConfigureAsync(IGamePackageRegister register)
    {
        register.RegisterSingleton<IDeckCount, RookDeckCount>();
        Core.DIFinishProcesses.GlobalDIAutoRegisterClass.RegisterNonSavedClasses(GetDIContainer);
        Core.DIFinishProcesses.SpecializedRegistrationHelpers.RegisterCommonMultplayerClasses(GetDIContainer);
        //if using misc deck, then remove this line of code.
        Core.DIFinishProcesses.AutoResetClass.RegisterAutoResets();
        //anything that needs to be registered will be here.
        return Task.CompletedTask;
    }

    protected override void FinishRegistrations(IGamePackageRegister register)
    {
        register.RegisterType<RookShellViewModel>(); //has to use interface part to make it work with source generators.
        Core.DIFinishProcesses.GlobalDIFinishClass.FinishDIRegistrations(GetDIContainer);
        Core.AutoResumeContexts.GlobalRegistrations.Register();
    }
}