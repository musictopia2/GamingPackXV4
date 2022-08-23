namespace BasicGameFrameworkLibrary.Core.SpecializedGameTypes.YahtzeeStyleHelpers.ViewModels;

public interface IScoresheetAction
{
    Task RowAsync(RowInfo row);
}