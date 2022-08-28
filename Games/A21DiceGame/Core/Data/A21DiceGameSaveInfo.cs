namespace A21DiceGame.Core.Data;
[SingletonGame]
public class A21DiceGameSaveInfo : BasicSavedDiceClass<SimpleDice, A21DiceGamePlayerItem>, IMappable, ISaveInfo
{
    public bool IsFaceOff { get; set; }
}