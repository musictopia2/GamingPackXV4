namespace DealCardGame.Blazor;
public class Bootstrapper(IStartUp starts, EnumGamePackageMode mode) : MultiplayerBasicBootstrapper<DealCardGameShellViewModel>(starts, mode)
{
    protected override Task RegisterTestsAsync()
    {
        //TestData!.CardsToPass = 10;
        //TestData.SaveOption = EnumTestSaveCategory.RestoreOnly;
        GetDIContainer.RegisterSingleton<ITestCardSetUp<DealCardGameCardInformation, DealCardGamePlayerItem>, RentCards>();
        //GetDIContainer.RegisterSingleton<ITestCardSetUp<DealCardGameCardInformation, DealCardGamePlayerItem>, BirthdayCards>();
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
public class RentCards : ITestCardSetUp<DealCardGameCardInformation, DealCardGamePlayerItem>
{
    Task ITestCardSetUp<DealCardGameCardInformation, DealCardGamePlayerItem>.SetUpTestHandsAsync(PlayerCollection<DealCardGamePlayerItem> playerList, IListShuffler<DealCardGameCardInformation> deckList)
    {
        int skip = 0;
        var player = playerList.GetSelf(); //has to be me that has to play the rent 2 color choice.
        DealCardGameCardInformation card;
        card = deckList.First(x => x.CardType == EnumCardType.ActionRent && x.FirstColorChoice == EnumColor.Black);
        player.StartUpList.Add(card);
        card = deckList.First(x => x.CardType == EnumCardType.PropertyRegular && x.MainColor == EnumColor.Black);
        player.StartUpList.Add(card);
        card = deckList.First(x => x.ActionCategory == EnumActionCategory.Birthday);
        player.StartUpList.Add(card); //try first with debt collector and then happy birthday.
        card = deckList.First(x => x.ActionCategory == EnumActionCategory.DebtCollector);
        player.StartUpList.Add(card);
        

        //var list = deckList.Where(x => x.ActionCategory == EnumActionCategory.DoubleRent).Take(2);
        //player.StartUpList.AddRange(list);
        foreach (var item in playerList)
        {
            //card = deckList.Where(x => x.ActionCategory == EnumActionCategory.JustSayNo).Skip(skip).Take(1).Single();
            //item.StartUpList.Add(card);
            if (item.PlayerCategory != EnumPlayerCategory.Self)
            {
                //each player will have the money ones guaranteed.
                card = deckList.Where(x => x.CardType == EnumCardType.Money && x.ClaimedValue == 1).Skip(skip).Take(1).Single();
                item.StartUpList.Add(card);
                card = deckList.Where(x => x.CardType == EnumCardType.Money && x.ClaimedValue == 2).Skip(skip).Take(1).Single();
                item.StartUpList.Add(card);
                card = deckList.Where(x => x.CardType == EnumCardType.Money && x.ClaimedValue == 3).Skip(skip).Take(1).Single();
                item.StartUpList.Add(card);
                card = deckList.Where(x => x.CardType == EnumCardType.Money && x.ClaimedValue == 4).Skip(skip).Take(1).Single();
                item.StartUpList.Add(card);
                card = deckList.Where(x => x.CardType == EnumCardType.Money && x.ClaimedValue == 5).Skip(skip).Take(1).Single();
                item.StartUpList.Add(card);
                skip++;
                if (skip > 1)
                {
                    break;
                }
            }


            //card = deckList.Where(x => x.ActionCategory == EnumActionCategory.Birthday).Skip(skip).Take(1).Single();
            //item.StartUpList.Add(card);
            //card = deckList.Where(x => x.ActionCategory == EnumActionCategory.DebtCollector).Skip(skip).Take(1).Single();
            //item.StartUpList.Add(card);

        }
        return Task.CompletedTask;
    }
}
public class BirthdayCards : ITestCardSetUp<DealCardGameCardInformation, DealCardGamePlayerItem>
{
    Task ITestCardSetUp<DealCardGameCardInformation, DealCardGamePlayerItem>.SetUpTestHandsAsync(PlayerCollection<DealCardGamePlayerItem> playerList, IListShuffler<DealCardGameCardInformation> deckList)
    {
        int skip = 0;
        foreach (var item in playerList)
        {
            var card = deckList.Where(x => x.ActionCategory == EnumActionCategory.JustSayNo).Skip(skip).Take(1).Single();
            item.StartUpList.Add(card);
            card = deckList.Where(x => x.CardType == EnumCardType.Money && x.ClaimedValue == 3).Skip(skip).Take(1).Single();
            item.StartUpList.Add(card);
            card = deckList.Where(x => x.CardType == EnumCardType.Money && x.ClaimedValue == 4).Skip(skip).Take(1).Single();
            item.StartUpList.Add(card);
            card = deckList.Where(x => x.ActionCategory == EnumActionCategory.Birthday).Skip(skip).Take(1).Single();
            item.StartUpList.Add(card);
            card = deckList.Where(x => x.ActionCategory == EnumActionCategory.DebtCollector).Skip(skip).Take(1).Single();
            item.StartUpList.Add(card);
            skip++;
        }
        return Task.CompletedTask;
    }
}
public class TestCards : ITestCardSetUp<DealCardGameCardInformation, DealCardGamePlayerItem>
{
    Task ITestCardSetUp<DealCardGameCardInformation, DealCardGamePlayerItem>.SetUpTestHandsAsync(PlayerCollection<DealCardGamePlayerItem> playerList, IListShuffler<DealCardGameCardInformation> deckList)
    {
        var player = playerList.GetSelf();
        var card = deckList.First(x => x.CardType == EnumCardType.ActionRent && x.AnyColor == true);
        player.StartUpList.Add(card);
        var list = deckList.Where(x => x.ActionCategory == EnumActionCategory.House).Take(1);
        player.StartUpList.AddRange(list);
        list = deckList.Where(x => x.ActionCategory == EnumActionCategory.Hotel).Take(1);
        player.StartUpList.AddRange(list);
        list = deckList.Where(x => x.MainColor == EnumColor.Brown && x.CardType == EnumCardType.PropertyRegular).Take(2);
        player.StartUpList.AddRange(list);

        
        //the other player has to play the birthday for testing.
        //player = playerList.GetSelf();
        //return Task.CompletedTask;
        if (playerList.Count > 2)
        {
            //list = deckList.Where(x => x.ActionCategory == EnumActionCategory.DoubleRent).Take(2);
            //player.StartUpList.AddRange(list);
            return Task.CompletedTask;
        }
        player = playerList.GetOnlyOpponent();
        if (player.PlayerCategory != EnumPlayerCategory.Computer)
        {
            list = deckList.Where(x => x.ActionCategory == EnumActionCategory.House).Skip(1).Take(1);
            player.StartUpList.AddRange(list);
            list = deckList.Where(x => x.ActionCategory == EnumActionCategory.Hotel).Skip(1).Take(1);
            player.StartUpList.AddRange(list);
            card = deckList.Where(x => x.CardType == EnumCardType.ActionRent && x.AnyColor == true).Skip(1).First();
            player.StartUpList.Add(card);
            //player.StartUpList.Add(card);
            //card = deckList.First(x => x.ActionCategory == EnumActionCategory.House);
            //player.StartUpList.Add(card);
            //card = deckList.First(x => x.ActionCategory == EnumActionCategory.Hotel);
            //player.StartUpList.Add(card);
            list = deckList.Where(x => x.MainColor == EnumColor.DarkBlue).Take(2);
            player.StartUpList.AddRange(list); //needs to have a monopoly so i can test the house and hotel.
            //list = deckList.Where(x => x.ActionCategory == EnumActionCategory.DoubleRent).Take(2);
            //player.StartUpList.AddRange(list);

            
        }
        //var card = deckList.First(x => x.ActionCategory == EnumActionCategory.Gos);

        return Task.CompletedTask;
    }
}