namespace BasicGameFrameworkLibrary.Core.SpecializedGameTypes.YahtzeeStyleHelpers.Logic;
public class YahtzeeEndRoundLogic<D> : IYahtzeeEndRoundLogic
    where D : SimpleDice, new()
{
    private readonly YahtzeeGameContainer<D> _gameContainer;
    private readonly ScoreContainer _scoreContainer;

    public YahtzeeEndRoundLogic(YahtzeeGameContainer<D> gameContainer,
        ScoreContainer scoreContainer
        )
    {
        _gameContainer = gameContainer;
        _scoreContainer = scoreContainer;
    }
    private async Task GameOverAsync()
    {
        _gameContainer.SingleInfo = _gameContainer.PlayerList!.OrderByDescending(items => items.Points).Take(1).Single();
        _gameContainer.SaveRoot!.Round = 0;
        _gameContainer.SaveRoot.RollNumber = 0;
        await _gameContainer.ShowWinAsync!.Invoke();
    }
    public bool IsGameOver => _scoreContainer.RowList.Where(items => items.RowSection == EnumRow.Regular)
        .All(xx => xx.HasFilledIn() == true);
    async Task IYahtzeeEndRoundLogic.StartNewRoundAsync()
    {
        if (_scoreContainer.RowList.Count == 0)
        {
            throw new CustomBasicException("The row list of the score container cannot be 0.  Rethink");
        }
        if (_gameContainer.WhoTurn == _gameContainer.SaveRoot!.Begins)
        {
            if (IsGameOver == true || _gameContainer.Test.ImmediatelyEndGame)
            {
                await GameOverAsync();
                return;
            }
            _gameContainer.SaveRoot.Round++;
        }
        await _gameContainer.StartNewTurnAsync!.Invoke();
    }
}