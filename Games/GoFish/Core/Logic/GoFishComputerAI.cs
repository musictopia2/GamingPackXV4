namespace GoFish.Core.Logic;
public static class GoFishComputerAI
{
    public static EnumRegularCardValueList NumberToAsk(GoFishSaveInfo saveRoot)
    {
        GoFishPlayerItem singleInfo = saveRoot.PlayerList.GetWhoPlayer();
        return singleInfo.MainHandList.GetRandomItem().Value;
    }
    public static BasicList<RegularSimpleCard> PairToPlay(GoFishSaveInfo saveRoot)
    {
        GoFishPlayerItem singleInfo = saveRoot.PlayerList.GetWhoPlayer();
        BasicList<RegularSimpleCard> output = new();
        foreach (var firstCard in singleInfo.MainHandList)
        {
            foreach (var secondCard in singleInfo.MainHandList)
            {
                if (firstCard.Deck != secondCard.Deck)
                {
                    if (firstCard.Value == secondCard.Value)
                    {
                        output.Add(firstCard);
                        output.Add(secondCard);
                        return output;
                    }
                }
            }
        }
        return new();
    }
}