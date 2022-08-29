﻿namespace ClueBoardGameBlazor;
public static class GlobalDetectiveGrid
{
    public static int RoomWidth { get; set; } = 100;
    public static int WeaponWidth { get; set; } = 100;
    public static int CharacterWidth { get; set; } = 125;
    public static int CellHeight { get; set; } = 25;
    public static int HeaderColumnHeight { get; set; } = 10;
    public static float HeaderFontSize => HeaderColumnHeight * .9f;
    public static float MainFontSize => CellHeight * .5f;
    public static string TargetGridWidth { get; set; } = "22vw";
    public static string TargetEnterGridWidth { get; set; } = "90vw";
    private static int GridWidth => RoomWidth + WeaponWidth + CharacterWidth;
    private static int MainHeight => CellHeight * 9;
    public static SizeF HeaderSize => new(GridWidth, HeaderColumnHeight);
    public static SizeF MainSize => new(GridWidth, MainHeight);
    public static RectangleF RoomHeaderLocation => new(0, 0, RoomWidth, HeaderColumnHeight);
    public static RectangleF CharacterHeaderLocation => new(RoomWidth, 0, CharacterWidth, HeaderColumnHeight);
    public static RectangleF WeaponHeaderLocation => new(RoomWidth + CharacterWidth, 0, WeaponWidth, HeaderColumnHeight);
}