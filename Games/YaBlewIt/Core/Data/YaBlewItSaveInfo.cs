namespace YaBlewIt.Core.Data;
[SingletonGame]
public class YaBlewItSaveInfo : BasicSavedCardClass<YaBlewItPlayerItem, YaBlewItCardInformation>, IMappable, ISaveInfo
{
    public BasicList<int> Claims { get; set; } = new();
    public DiceList<EightSidedDice> DiceList { get; set; } = new();
    public EnumGameStatus PreviousStatus { get; set; } = EnumGameStatus.None; //this is used so if ending turn and somebody plays something can know what to do with it.
    public EnumGameStatus OldestStatus { get; set; } = EnumGameStatus.None; //this is needed so if there is a fire, then that becomes previuousstatus.  since other things has to happen to resolve fires.
    public EnumGameStatus GameStatus { get; set; }
    public bool PlayedFaulty { get; set; } //if you play faulty, then you have to roll again period. (can't change mind about safe cards either).
    public bool DrewExtra { get; set; }
    //looks like you can protect more than one color though.
    public BasicList<EnumColors> ProtectedColors { get; set; } = new();
    public BasicList<int> SafeList { get; set; } = new(); //i like the idea of just integers.  this means if you need to get it back (no reshuffling), can just populate and generate a new card for player if necessary
}