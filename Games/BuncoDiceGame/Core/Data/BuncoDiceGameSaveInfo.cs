namespace BuncoDiceGame.Core.Data;
[SingletonGame]
public class BuncoDiceGameSaveInfo : IMappable, ISaveInfo
{
    public PlayerCollection<PlayerItem> PlayerList { get; set; } = new PlayerCollection<PlayerItem>();
    public StatisticsInfo ThisStats { get; set; } = new StatisticsInfo(); //making this first so i can step through via source generator easier.
    public DiceList<SimpleDice> DiceList { get; set; } = new DiceList<SimpleDice>();//i think
    public PlayOrderClass? PlayOrder { get; set; }
    public int WhatSet { get; set; }
    public int WhatNumber { get; set; }
    public bool DidHaveBunco { get; set; }
    public bool SameTable { get; set; }
    public int TurnNum { get; set; } //i think this is still needed.
    public bool HadBunco { get; set; }
    public bool HasRolled { get; set; }
    public bool MaxedRolls { get; set; }
}