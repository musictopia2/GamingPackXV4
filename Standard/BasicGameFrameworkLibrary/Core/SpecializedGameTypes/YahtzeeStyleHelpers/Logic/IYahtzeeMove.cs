namespace BasicGameFrameworkLibrary.Core.SpecializedGameTypes.YahtzeeStyleHelpers.Logic;
public interface IYahtzeeMove
{
    Task MakeMoveAsync(RowInfo row);
}