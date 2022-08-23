﻿namespace BasicGameFrameworkLibrary.Core.RegularDeckOfCards;

public interface IRegularDeckInfo : IDeckCount //may have to know about that (well see)
{
    int HowManyDecks { get; }
    bool UseJokers { get; }
    int GetExtraJokers { get; } //there is the chance of having extra jokers.  could be helpful for the cousins game.
    int LowestNumber { get; }
    int HighestNumber { get; } //for this case, show 14 for ace.  however, for aces, will delegate to the populate ace values
    BasicList<ExcludeRCard> ExcludeList { get; } //i think will be readonly.  if i am wrong, rethink
    BasicList<EnumSuitList> SuitList { get; }
}