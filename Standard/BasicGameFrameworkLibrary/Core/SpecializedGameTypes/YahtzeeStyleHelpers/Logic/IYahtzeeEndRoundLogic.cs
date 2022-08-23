namespace BasicGameFrameworkLibrary.Core.SpecializedGameTypes.YahtzeeStyleHelpers.Logic;

public interface IYahtzeeEndRoundLogic
{
    Task StartNewRoundAsync();
    bool IsGameOver { get; }
}