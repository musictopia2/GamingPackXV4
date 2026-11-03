namespace Phase10.Core.Data;
[SingletonGame]
public class Phase10SaveInfo : BasicSavedCardClass<Phase10PlayerItem, Phase10CardInformation>, IMappable, ISaveInfo
{
    public BasicList<SavedSet> SetList { get; set; } = [];
    public bool CompletedPhase { get; set; }
    public bool Skips { get; set; }
    public bool IsTie { get; set; }
}