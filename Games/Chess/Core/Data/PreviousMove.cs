﻿namespace Chess.Core.Data;
public class PreviousMove // 0 and 0 means no move was made for previous move
{
    public int SpaceFrom { get; set; }
    public int SpaceTo { get; set; }
    public string PlayerColor { get; set; } = cs1.Transparent; // this is the color for the previous player.
}