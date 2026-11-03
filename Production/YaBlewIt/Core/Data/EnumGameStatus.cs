namespace YaBlewIt.Core.Data;
public enum EnumGameStatus
{
    None,
    Beginning,
    ProspectorDraws,
    ProspectorStarts,
    ProspectorContinues, //this means everybody tried.  this means cannot play jumper
    ResolveFire,
    MinerRolling,
    GamblingDecision,
    StartGambling,
    FinishGambling,
    Safe,
    EndingTurn
}