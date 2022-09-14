namespace MonopolyCardGame.Blazor;
public class TestCards : ITestCardSetUp<MonopolyCardGameCardInformation, MonopolyCardGamePlayerItem>
{
    Task ITestCardSetUp<MonopolyCardGameCardInformation, MonopolyCardGamePlayerItem>.SetUpTestHandsAsync(PlayerCollection<MonopolyCardGamePlayerItem> playerList, IListShuffler<MonopolyCardGameCardInformation> deckList)
    {
        var first = playerList.First();

        var second = playerList.Last();
        var cards = deckList.Where(x => x.WhatCard == EnumCardType.IsMr).Take(2);
        first.StartUpList.AddRange(cards);
        cards = deckList.Where(x => x.WhatCard == EnumCardType.IsMr).Skip(2).Take(1);
        second.StartUpList.AddRange(cards);
        //var player = playerList.GetSelf();
        //var cards = deckList.Where(x => x.WhatCard == EnumCardType.IsChance).Take(1);
        //player.StartUpList.AddRange(cards);
        return Task.CompletedTask;
    }
}