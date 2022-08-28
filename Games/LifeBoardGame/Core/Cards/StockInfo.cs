namespace LifeBoardGame.Core.Cards;
public class StockInfo : LifeBaseCard
{
    public StockInfo()
    {
        CardCategory = EnumCardCategory.Stock;
    }
    public int Value { get; set; }
}