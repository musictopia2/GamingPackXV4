namespace BasicGameFrameworkLibrary.Core.SpecializedGameTypes.YahtzeeStyleHelpers.Logic;
public class YahtzeeEndRoundLogic<D>(YahtzeeGameContainer<D> gameContainer,
    ScoreContainer scoreContainer
        ) : IYahtzeeEndRoundLogic
    where D : SimpleDice, new()
{
    private async Task GameOverAsync()
    {
        gameContainer.SingleInfo = gameContainer.PlayerList!.OrderByDescending(items => items.Points).Take(1).Single();
        gameContainer.SaveRoot!.Round = 0;
        gameContainer.SaveRoot.RollNumber = 0;
        await gameContainer.ShowWinAsync!.Invoke();
    }
    public bool IsGameOver => scoreContainer.RowList.Where(items => items.RowSection == EnumRow.Regular)
        .All(xx => xx.HasFilledIn() == true);
    async Task IYahtzeeEndRoundLogic.StartNewRoundAsync()
    {
        if (scoreContainer.RowList.Count == 0)
        {
            throw new CustomBasicException("The row list of the score container cannot be 0.  Rethink");
        }
        if (gameContainer.WhoTurn == gameContainer.SaveRoot!.Begins)
        {
            if (IsGameOver == true || gameContainer.Test.ImmediatelyEndGame)
            {
                await GameOverAsync();
                return;
            }
            gameContainer.SaveRoot.Round++;
        }
        await gameContainer.StartNewTurnAsync!.Invoke();
    }
}