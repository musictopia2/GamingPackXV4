namespace OldMaid.Core.Data;
[SingletonGame]
public class OldMaidSaveInfo : BasicSavedCardClass<OldMaidPlayerItem, RegularSimpleCard>, IMappable, ISaveInfo
{
    public bool RemovePairs { get; set; }
    public bool AlreadyChoseOne { get; set; }
}