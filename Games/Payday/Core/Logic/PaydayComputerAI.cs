namespace Payday.Core.Logic;
public static class PaydayComputerAI
{
    private static BasicList<PaydayPlayerItem> GetPossiblePlayerList(PaydayGameContainer mainGame, PaydayVMData model)
    {
        BasicList<PaydayPlayerItem> output = new();
        BasicList<string> tempList = model.PopUpList!.TextList.Select(items => items.DisplayText).ToBasicList();
        tempList.ForEach(Name =>
        {
            output.Add(mainGame.PlayerList!.Single(xx => xx.NickName == Name));
        });
        return output;
    }
    public static string PlayerChosen(PaydayGameContainer mainGame, PaydayVMData model)
    {
        if (model.PopUpList!.Count() == 1)
        {
            return model.PopUpList.GetText(1);
        }
        BasicList<PaydayPlayerItem> tempList = GetPossiblePlayerList(mainGame, model);
        MailCard thisMail = model.MailPile.GetCardInfo();
        return thisMail.MailType switch
        {
            EnumMailType.MadMoney => tempList.OrderByDescending(xx => xx.NetIncome()).Take(1).Single().NickName,
            EnumMailType.PayNeighbor => tempList.OrderBy(xx => xx.NetIncome()).Take(1).Single().NickName,
            _ => throw new CustomBasicException($"Must be madmoney or payneighbor, not {thisMail.CardCategory}"),
        };
    }
    public static bool LandDeal(PaydayMainGameClass mainGame)
    {
        return !mainGame.SingleInfo!.Hand.Any(items => items.CardCategory == EnumCardCategory.Deal);
    }
    public static bool PurchaseDeal(PaydayMainGameClass mainGame)
    {
        int dealCount = mainGame.SingleInfo!.Hand.Count(items => items.CardCategory == EnumCardCategory.Deal);
        if (mainGame.SingleInfo.CurrentMonth == mainGame.SaveRoot!.MaxMonths)
        {
            if (mainGame.SingleInfo.DayNumber > 13)
            {
                return false;
            }
            if (dealCount > 1)
            {
                return false;
            }
            return true;
        }
        if (mainGame.SingleInfo.CurrentMonth + 1 == mainGame.SaveRoot.MaxMonths)
        {
            if (dealCount > 3)
            {
                return false;
            }
            return true;
        }
        if (dealCount > 4)
        {
            return false;
        }
        return true;
    }
    public static int NumberChosen(PaydayVMData model)
    {
        int index = model.PopUpList!.ItemToChoose(true);
        int output;
        do
        {
            output = int.Parse(model.PopUpList.GetText(index));
            if (output > -1)
            {
                return output; //the computer will always have a chance of playing lottery.
            }

        } while (true);
    }
    public static int BuyerSelected(PaydayMainGameClass mainGame)
    {
        var thisList = mainGame.SingleInfo!.Hand.GetMailOrDealList<DealCard>(EnumCardCategory.Deal);
        return thisList.OrderByDescending(xx => xx.Value).ThenByDescending(xx => xx.Profit()).Take(1).Single().Deck;
    }
}