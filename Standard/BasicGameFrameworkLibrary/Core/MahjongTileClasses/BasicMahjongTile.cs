namespace BasicGameFrameworkLibrary.Core.MahjongTileClasses;
public abstract class BasicMahjongTile : SimpleDeckObject, IMahjongTileInfo
{
    public enum EnumDirectionType
    {
        IsNorth = 1,
        IsSouth = 2,
        IsWest = 3,
        IsEast = 4,
        IsNoDirection = 0
    }
    public enum EnumColorType
    {
        IsRed = 1,
        IsGreen = 2,
        IsWhite = 3,
        IsNoColor = 4
    }
    public enum EnumBonusType
    {
        IsNoBonus = 0,
        IsSeason = 1,
        IsFlower = 2
    }
    public enum EnumNumberType
    {
        IsCircle = 1,
        IsBamboo = 2,
        IsCharacter = 3,
        IsNoNumber = 0
    }
    public int Index { get; set; } // this is needed to get the proper image when it comes to drawing.
    public int NumberUsed { get; set; }
    public EnumColorType WhatColor { get; set; }
    public EnumBonusType WhatBonus { get; set; }
    public EnumNumberType WhatNumber { get; set; }
    public EnumDirectionType WhatDirection { get; set; }
    public float Left { get; set; }
    public float Top { get; set; }
    public bool NeedsLeft { get; set; }
    public bool NeedsTop { get; set; }
    public bool NeedsRight { get; set; }
    public bool NeedsBottom { get; set; }
}