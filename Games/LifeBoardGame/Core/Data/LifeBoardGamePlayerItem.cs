namespace LifeBoardGame.Core.Data;
[UseScoreboard]
public partial class LifeBoardGamePlayerItem : PlayerBoardGame<EnumColorChoice>
{
    public override bool DidChooseColor => Color.IsNull == false && Color != EnumColorChoice.None;
    public override void Clear()
    {
        Color = EnumColorChoice.None;
    }
    public DeckRegularDict<LifeBaseCard> Hand { get; set; } = new(); //this time, can use the standard hand.
    public EnumStart OptionChosen { get; set; }
    public int Position { get; set; } // where you are at on the board.
    public EnumFinal LastMove { get; set; }
    public int Distance { get; set; }
    public EnumGender Gender { get; set; }
    public BasicList<EnumGender> ChildrenList { get; set; } = new();
    [ScoreColumn]
    public decimal Loans { get; set; }
    [ScoreColumn]
    public decimal Salary { get; set; }
    [ScoreColumn]
    public decimal MoneyEarned { get; set; }
    [ScoreColumn]
    public int FirstStock { get; set; }
    [ScoreColumn]
    public int SecondStock { get; set; }
    [ScoreColumn]
    public bool CarIsInsured { get; set; }
    [ScoreColumn]
    public bool HouseIsInsured { get; set; }
    [ScoreColumn]
    public bool DegreeObtained { get; set; }
    [ScoreColumn]
    public int TilesCollected { get; set; }
    [ScoreColumn]
    public string HouseName { get; set; } = "";
    [ScoreColumn]
    public string Career1 { get; set; } = "";
    [ScoreColumn]
    public string Career2 { get; set; } = "";
    public BasicList<TileInfo> TileList { get; set; } = new();
    public EnumTurnInfo WhatTurn { get; set; }
    public bool Married { get; set; }
}