﻿namespace FillOrBust.Core.Data;
public enum EnumGameStatusList
{
    DrawCard = 1,
    EndTurn = 2,
    RollDice = 3,
    ChoosePlay = 4, // this means it has a choice to draw or play card
    ChooseDice = 5, // this means it needs to choose at least one scoring dice
    ChooseDraw = 6, // this means the fill was successful.  therefore, gets to choose on whether to draw or end turn
    ChooseRoll = 7, // this means it has a choice on whether to roll or not
}