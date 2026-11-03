namespace Kismet.Core.Logic;
[SingletonGame]
public class KismetScoreProcesses(YahtzeeGameContainer<KismetDice> gameContainer,
    ScoreContainer scoreContainer,
    YahtzeeVMData<KismetDice> model
        ) : IYahtzeeStyle
{
    BasicList<string> IYahtzeeStyle.GetBottomText =>
    [
        "2 Pair Same Color",
        "3 Of A Kind",
        "Straight (30)",
        "Flush All Same Color (35)",
        "Full House",
        "Full House Same Color",
        "4 Of A Kind",
        "Yarborough",
        "Kismet (5 Of A Kind)"
    ];
    bool IYahtzeeStyle.HasExceptionFor5Kind => true;
    int IYahtzeeStyle.BottomDescriptionWidth => 700;
    int IYahtzeeStyle.BonusAmount(int topScore)
    {
        if (topScore < 63)
        {
            return 0;
        }
        if (topScore <= 70)
        {
            return 35;
        }
        if (topScore <= 77)
        {
            return 55;
        }
        return 75;
    }
    void IYahtzeeStyle.Extra5OfAKind()
    {
        var firstList = gameContainer.PlayerList!.ToBasicList();
        firstList.RemoveSpecificItem(gameContainer.SingleInfo!);
        firstList.ForEach(Items => Items.MissNextTurn = true);
    }

    BasicList<DiceInformation> IYahtzeeStyle.GetDiceList()
    {
        var firstList = model.Cup!.DiceList.ToBasicList();
        BasicList<DiceInformation> output = [];
        firstList.ForEach(items =>
        {
            DiceInformation thisDice = new();
            thisDice.Value = items.Value;
            thisDice.Color = items.DotColor;
            output.Add(thisDice);
        });
        return output;
    }
    private bool HasFlush()
    {
        int count = scoreContainer.DiceList.MaximumDuplicates(x => x.Color);
        return count == 5;
    }
    private bool HasTwoPairSameColor()
    {
        if (scoreContainer.HasFullHouse() == true && HasFlush() == true)
        {
            return true;
        }
        if (scoreContainer.HasKind(5))
        {
            return true;
        }
        var tempList = scoreContainer.DiceList.GroupOrderDescending(x => x.Color);
        if (tempList.Count() > 2)
        {
            return false;
        }
        if (tempList.First().Count() < 4)
        {
            return false; //i did that in the old.  if that is not correct, rethink.
        }
        var first = scoreContainer.DiceList.Where(x => x.Color == tempList.First().Key)
            .GroupOrderDescending(x => x.Value).ToBasicList();
        if (first.First().Count() == 4)
        {
            return true;
        }
        if (first.First().Count() != 2)
        {
            return false;
        }
        if (first[1].Count() != 2)
        {
            return false;
        }
        return true;
    }
    void IYahtzeeStyle.PopulateBottomScores()
    {
        if (HasTwoPairSameColor() == true && scoreContainer.RowList[9].HasFilledIn() == false)
        {
            scoreContainer.RowList[9].Possible = scoreContainer.CalculateDiceTotal();
        }
        if (scoreContainer.HasKind(3) == true && scoreContainer.RowList[10].HasFilledIn() == false)
        {
            scoreContainer.RowList[10].Possible = scoreContainer.CalculateDiceTotal();
        }
        if (scoreContainer.HasStraight(false) == true && scoreContainer.RowList[11].HasFilledIn() == false)
        {
            scoreContainer.RowList[11].Possible = 30;//
        }
        if (HasFlush() == true && scoreContainer.RowList[12].HasFilledIn() == false)
        {
            scoreContainer.RowList[12].Possible = 35;
        }
        if (scoreContainer.HasFullHouse() == true && scoreContainer.RowList[13].HasFilledIn() == false)
        {
            scoreContainer.RowList[13].Possible = scoreContainer.CalculateDiceTotal() + 15;
        }
        if (scoreContainer.HasFullHouse() == true && HasFlush() == true && scoreContainer.RowList[14].HasFilledIn() == false)
        {
            scoreContainer.RowList[14].Possible = scoreContainer.CalculateDiceTotal() + 20;
        }
        if (scoreContainer.HasKind(4) == true && scoreContainer.RowList[15].HasFilledIn() == false)
        {
            scoreContainer.RowList[15].Possible = scoreContainer.CalculateDiceTotal() + 25;
        }
        if (scoreContainer.RowList[16].HasFilledIn() == false)
        {
            scoreContainer.RowList[16].Possible = scoreContainer.CalculateDiceTotal();
        }
        if (scoreContainer.HasAllFive() == true && scoreContainer.RowList[17].HasFilledIn() == false)
        {
            scoreContainer.RowList[17].Possible = scoreContainer.CalculateDiceTotal() + 50;
        }
    }
}