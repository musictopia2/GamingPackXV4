namespace LifeBoardGame.Core.Cards;
public class CareerInfo : LifeBaseCard
{
    public CareerInfo()
    {
        CardCategory = EnumCardCategory.Career;
    }
    public EnumCareerType Career { get; set; }
    public string Title { get; set; } = "";
    public EnumPayScale Scale1 { get; set; }
    public EnumPayScale Scale2 { get; set; }
    public bool DegreeRequired { get; set; }
    public string Description { get; set; } = "";
}