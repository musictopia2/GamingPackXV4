namespace Savannah.Core.Piles;
public class StockViewModel : StockPileVM<RegularSimpleCard>
{
    public StockViewModel(CommandContainer command) : base(command) { }
    public override string NextCardInStock()
    {
        if (DidGoOut() == true)
        {
            return "0";
        }
        var thisCard = GetCard();
        return thisCard.Value.Name;
    }
    protected override string TextToAppear => "Reserve";
}