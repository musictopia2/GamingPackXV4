namespace Cribbage.Core.Data;
public class CribbageCombos
{
    public string Description = "";
    public EnumPlayType WhenUsed;
    public int NumberNeeded;
    public int CardsToUse;
    public int NumberInStraight;
    public bool IsFlush;
    public int NumberForKind;
    public EnumCribbageEquals WhatEqual;
    public int Points;
    public bool DoublePairNeeded;
    public EnumScoreGroups Group;
    public bool JackStarter;
    public bool JackSameSuitAsStarter;
    public BasicList<CribbageCard> Hand = new(); //try to use cribbage card now (?)
}