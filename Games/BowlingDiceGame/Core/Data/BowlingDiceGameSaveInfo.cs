namespace BowlingDiceGame.Core.Data;
[SingletonGame]
public class BowlingDiceGameSaveInfo : BasicSavedGameClass<BowlingDiceGamePlayerItem>, IMappable, ISaveInfo
{
    public bool IsExtended { get; set; }
    public int WhichPart { get; set; }
    public int WhatFrame { get; set; }
    public string DiceData { get; set; } = "";
}