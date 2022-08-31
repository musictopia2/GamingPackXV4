namespace MonopolyCardGame.Blazor;
public class TestCards : ITestCardSetUp<MonopolyCardGameCardInformation, MonopolyCardGamePlayerItem>
{
    Task ITestCardSetUp<MonopolyCardGameCardInformation, MonopolyCardGamePlayerItem>.SetUpTestHandsAsync(PlayerCollection<MonopolyCardGamePlayerItem> playerList, IListShuffler<MonopolyCardGameCardInformation> deckList)
    {
        var player = playerList.GetSelf();
        var cards = deckList.Where(x => x.WhatCard == EnumCardType.IsChance).Take(1);
        player.StartUpList.AddRange(cards);
        return Task.CompletedTask;
    }
}