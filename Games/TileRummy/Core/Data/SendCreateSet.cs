namespace TileRummy.Core.Data;
public class SendCreateSet
{
    public EnumWhatSets WhatSet { get; set; }
    public int FirstNumber { get; set; }
    public int SecondNumber { get; set; }
    public BasicList<int> CardList { get; set; } = new();
}