namespace Flinch.Core.Piles;
public class StockViewModel(CommandContainer command) : StockPileVM<FlinchCardInformation>(command)
{
    public override string NextCardInStock()
    {
        if (DidGoOut() == true)
        {
            return "0";
        }
        var thisCard = GetCard();
        return thisCard.Display;
    }
}