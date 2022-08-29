namespace Concentration.Core.Logic;
public static class ComputerAI
{
    public static int CardToTry(ConcentrationMainGameClass mainGame, ConcentrationVMData model, ConcentrationGameContainer container)
    {
        DeckRegularDict<RegularSimpleCard> computerList = mainGame.SaveRoot!.ComputerList;
        DeckRegularDict<RegularSimpleCard> output;
        if (computerList.Count == 0)
        {
            output = model.GameBoard1!.CardsLeft();
            return output.GetRandomItem().Deck;
        }
        output = PairToTry(computerList, container);
        if (output.Count == 0)
        {
            output = model.GameBoard1!.CardsLeft();
            return output.GetRandomItem().Deck;
        }
        bool rets1 = model.GameBoard1!.WasSelected(output.First().Deck);
        bool rets2 = model.GameBoard1.WasSelected(output.Last().Deck);
        if (rets1 == true && rets2 == true)
        {
            throw new CustomBasicException("Both items cannot be selected");
        }
        if (rets1 == true)
        {
            return output.Last().Deck;
        }
        return output.First().Deck;
    }
    private static DeckRegularDict<RegularSimpleCard> PairToTry(DeckRegularDict<RegularSimpleCard> computerList, ConcentrationGameContainer container)
    {
        DeckRegularDict<RegularSimpleCard> output = new();

        //have a chance it pretend like it can't remember (no fun if computer wins all the time for having perfect memory).
        //tried 20 but remembered too often.  then tried 30.  that was a good balance.
        bool rets = container.Random.NextBool(30); //tried 70 percent chance it will forget but was too high.
        if (rets == true)
        {
            return new(); //pretend like it can't remember.
        }

        foreach (var firstCard in computerList)
        {
            foreach (var secondCard in computerList)
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
        return output;
    }
}