namespace Bingo.Core.Data;
public class BingoItem : IBasicSpace
{
    public Vector Vector { get; set; }
    public int WhatValue { get; set; }
    public string Letter { get; set; } = "";
    public bool DidGet { get; set; }
    public void ClearSpace()
    {
        DidGet = false;
    }
    public bool IsFilled()
    {
        return DidGet;
    }
}