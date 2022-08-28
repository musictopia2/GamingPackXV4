namespace LifeBoardGame.Core.Cards;
public class HouseInfo : LifeBaseCard
{
    public HouseInfo()
    {
        CardCategory = EnumCardCategory.House;
    }
    public EnumHouseType HouseCategory { get; set; }
    public string Title { get; set; } = "";
    public string Description { get; set; } = "";
    public decimal HousePrice { get; set; }
    public decimal InsuranceCost { get; set; }
    public Dictionary<int, decimal> SellingPrices { get; set; } = new();
}