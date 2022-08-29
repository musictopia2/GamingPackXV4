namespace Concentration.Core.Data;
[SingletonGame]
public class ConcentrationSaveInfo : BasicSavedCardClass<ConcentrationPlayerItem, RegularSimpleCard>, IMappable, ISaveInfo
{
    public BasicList<BasicPileInfo<RegularSimpleCard>> BoardList { get; set; } = new();
    public DeckRegularDict<RegularSimpleCard> ComputerList { get; set; } = new();
}