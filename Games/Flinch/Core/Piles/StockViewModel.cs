namespace Flinch.Core.Piles;
public class StockViewModel : StockPileVM<FlinchCardInformation>
{
    public StockViewModel(CommandContainer command) : base(command) { }
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