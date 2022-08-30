namespace CoveredUp.Blazor;
//used this to test the cards so i can duplicate the bug
//public class TestCards : ITestCardSetUp<RegularSimpleCard, CoveredUpPlayerItem>
//{
//    Task ITestCardSetUp<RegularSimpleCard, CoveredUpPlayerItem>.SetUpTestHandsAsync(PlayerCollection<CoveredUpPlayerItem> playerList, IListShuffler<RegularSimpleCard> deckList)
//    {
//        var player = playerList.GetSelf();
//        var list1 = deckList.Where(x => x.Value == EnumRegularCardValueList.Nine).Take(4).ToBasicList();

//        //0, 1, 4, 5
//        //player.StartUpList.AddRange(list);
//        var list2 = deckList.Where(x => x.Value != EnumRegularCardValueList.Nine).Take(4).ToBasicList();
//        //player.StartUpList.AddRange(list);

//        player.StartUpList.AddRange(list1.Take(2));

//        player.StartUpList.AddRange(list2.Take(2));
//        player.StartUpList.AddRange(list1.Skip(2).Take(2));

//        player.StartUpList.AddRange(list2.Skip(2).Take(2));


//        return Task.CompletedTask;
//    }
//}