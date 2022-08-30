namespace Savannah.Core.Data;
[SingletonGame]
public class SavannahSaveInfo : BasicSavedCardClass<SavannahPlayerItem, RegularSimpleCard>, IMappable, ISaveInfo
{
    public DiceList<SimpleDice> DiceList { get; set; } = new();
    public bool ChoseOtherPlayer { get; set; }
    public int TimesReshuffled { get; set; }
    public BasicList<BasicPileInfo<RegularSimpleCard>> PublicPileList { get; set; } = new();
}