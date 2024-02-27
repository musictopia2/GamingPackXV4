namespace MonopolyCardGame.Blazor;
public class TestCards : ITestCardSetUp<MonopolyCardGameCardInformation, MonopolyCardGamePlayerItem>
{
    Task ITestCardSetUp<MonopolyCardGameCardInformation, MonopolyCardGamePlayerItem>.SetUpTestHandsAsync(PlayerCollection<MonopolyCardGamePlayerItem> playerList, IListShuffler<MonopolyCardGameCardInformation> deckList)
    {
        //var first = playerList.First();

        //var second = playerList.Last();
        //var cards = deckList.Where(x => x.WhatCard == EnumCardType.IsMr).Take(2);
        //first.StartUpList.AddRange(cards);
        //cards = deckList.Where(x => x.WhatCard == EnumCardType.IsMr).Skip(2).Take(1);
        //second.StartUpList.AddRange(cards);
        var player = playerList.GetSelf();



        //var cards = deckList.Where(x => x.WhatCard == EnumCardType.IsChance).Take(1);
        //player.StartUpList.AddRange(cards);



        //player = playerList.GetOnlyOpponent();


        //cards = deckList.Where(x => x.WhatCard == EnumCardType.IsHotel).Take(1);
        //player.StartUpList.AddRange(cards);
        //4.Times(y =>
        //{
        //    if (y != 3)
        //    {
        //        cards = deckList.Where(x => x.WhatCard == EnumCardType.IsHouse && x.HouseNum == y).Take(1);
        //        player.StartUpList.AddRange(cards);
        //    }
        //});
        //cards = deckList.Where(x => x.WhatCard == EnumCardType.IsToken).Take(1);
        //player.StartUpList.AddRange(cards);
        //cards = deckList.Where(x => x.WhatCard == EnumCardType.IsToken).Take(2);
        //player.StartUpList.AddRange(cards);
        var cards = deckList.Where(x => x.Money == 300 && x.WhatCard == EnumCardType.IsProperty).Take(3);
        player.StartUpList.AddRange(cards);
        cards = deckList.Where(x => x.Money == 250 && x.WhatCard == EnumCardType.IsProperty).Take(3);
        player.StartUpList.AddRange(cards);
        cards = deckList.Where(x => x.Money == 400 && x.WhatCard == EnumCardType.IsProperty).Take(2);
        player.StartUpList.AddRange(cards);
        cards = deckList.Where(x => x.Money == 50 && x.WhatCard == EnumCardType.IsProperty).Take(2);
        player.StartUpList.AddRange(cards);
        //var cards = deckList.Where(x => x.WhatCard == EnumCardType.IsGo).Take(2);
        //player.StartUpList.AddRange(cards);
        return Task.CompletedTask;
    }
}