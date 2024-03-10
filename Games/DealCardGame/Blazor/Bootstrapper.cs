namespace DealCardGame.Blazor;
public class Bootstrapper(IStartUp starts, EnumGamePackageMode mode) : MultiplayerBasicBootstrapper<DealCardGameShellViewModel>(starts, mode)
{
    protected override Task RegisterTestsAsync()
    {
        TestData!.CardsToPass = 12;
        //TestData.SaveOption = EnumTestSaveCategory.RestoreOnly;
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
        var card = deckList.First(x => x.ActionCategory == EnumActionCategory.Birthday);
        player.StartUpList.Add(card);
        card = deckList.First(x => x.ActionCategory == EnumActionCategory.DebtCollector);
        player.StartUpList.Add(card);

        card = deckList.First(x => x.ActionCategory == EnumActionCategory.DealBreaker);
        player.StartUpList.Add(card); //so i can steal to see what happens.

        


        //the other player has to play the birthday for testing.
        //player = playerList.GetSelf();

        player = playerList.GetOnlyOpponent();
        if (player.PlayerCategory != EnumPlayerCategory.Computer)
        {
            var list = deckList.Where(x => x.ActionCategory == EnumActionCategory.House).Take(1);
            player.StartUpList.AddRange(list);
            list = deckList.Where(x => x.ActionCategory == EnumActionCategory.Hotel).Take(1);
            player.StartUpList.AddRange(list);
            //player.StartUpList.Add(card);
            //card = deckList.First(x => x.ActionCategory == EnumActionCategory.House);
            //player.StartUpList.Add(card);
            //card = deckList.First(x => x.ActionCategory == EnumActionCategory.Hotel);
            //player.StartUpList.Add(card);
            list = deckList.Where(x => x.MainColor == EnumColor.Yellow).Take(3);
            player.StartUpList.AddRange(list); //needs to have a monopoly so i can test the house and hotel.
        }
        //var card = deckList.First(x => x.ActionCategory == EnumActionCategory.Gos);
        
        return Task.CompletedTask;
    }
}