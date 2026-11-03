namespace LifeBoardGame.Core.Data;
[UseScoreboard]
public partial class LifeBoardGamePlayerItem : PlayerBoardGame<EnumColorChoice>
{
    public override bool DidChooseColor => Color.IsNull == false && Color != EnumColorChoice.None;
    public override void Clear()
    {
        Color = EnumColorChoice.None;
        Gender = EnumGender.None;
    }
    public DeckRegularDict<LifeBaseCard> Hand { get; set; } = []; //this time, can use the standard hand.
    public EnumStart OptionChosen { get; set; }
    public int Position { get; set; } // where you are at on the board.
    public EnumFinal LastMove { get; set; }
    public int Distance { get; set; }
    public EnumGender Gender { get; set; } = EnumGender.None;
    public BasicList<EnumGender> ChildrenList { get; set; } = [];
    [ScoreColumn]
    public partial decimal Loans { get; set; }
    [ScoreColumn]
    public partial decimal Salary { get; set; }
    [ScoreColumn]
    public partial decimal MoneyEarned { get; set; }
    [ScoreColumn]
    public partial int FirstStock { get; set; }
    [ScoreColumn]
    public partial int SecondStock { get; set; }
    [ScoreColumn]
    public partial bool CarIsInsured { get; set; }
    [ScoreColumn]
    public partial bool HouseIsInsured { get; set; }
    [ScoreColumn]
    public partial bool DegreeObtained { get; set; }
    [ScoreColumn]
    public partial int TilesCollected { get; set; }
    [ScoreColumn]
    public partial string HouseName { get; set; } = "";
    [ScoreColumn]
    public partial string Career1 { get; set; } = "";
    [ScoreColumn]
    public partial string Career2 { get; set; } = "";
    public BasicList<TileInfo> TileList { get; set; } = [];
    public EnumTurnInfo WhatTurn { get; set; }
    public bool Married { get; set; }
}