namespace Payday.Core.Data;
public enum EnumMailType
{
    None, //so the source generator can put none for a fresh mail.
    Bill = 1,
    Charity = 2,
    MadMoney = 3,
    MonsterCharge = 4,
    MoveAhead = 5,
    PayNeighbor = 6
}