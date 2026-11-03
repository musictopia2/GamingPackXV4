namespace GermanWhist.Core.Data;
[SingletonGame]
public class GermanWhistSaveInfo : BasicSavedTrickGamesClass<EnumSuitList, GermanWhistCardInformation, GermanWhistPlayerItem>, IMappable, ISaveInfo
{
    public bool WasEnd { get; set; }
}