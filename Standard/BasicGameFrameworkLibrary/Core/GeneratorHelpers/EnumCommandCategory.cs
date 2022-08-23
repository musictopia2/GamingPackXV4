namespace BasicGameFrameworkLibrary.Core.GeneratorHelpers;

public enum EnumCommandCategory
{
    Plain = 1,
    Game,
    Limited, //this means starts as limited.  used for games like clue board game for clicking on your clues.
    OutOfTurn,
    Open,
    Control,
    Old //this means its not tied to commandcontainer.  means you can click it even if all else is disabled.
}