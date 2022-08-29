namespace GoFish.Core.Data;
[SingletonGame]
public class GoFishSaveInfo : BasicSavedCardClass<GoFishPlayerItem, RegularSimpleCard>, IMappable, ISaveInfo
{
    public bool NumberAsked { get; set; }
    public bool RemovePairs { get; set; }
}