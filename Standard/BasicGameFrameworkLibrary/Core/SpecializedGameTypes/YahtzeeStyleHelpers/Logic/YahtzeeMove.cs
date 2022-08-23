namespace BasicGameFrameworkLibrary.Core.SpecializedGameTypes.YahtzeeStyleHelpers.Logic;

public class YahtzeeMove<D> : IYahtzeeMove, IMoveNM
    where D : SimpleDice, new()
{
    private readonly ScoreContainer _scoreContainer;
    private readonly IScoreLogic _scoreLogic;
    private readonly YahtzeeVMData<D> _model;
    private readonly IYahtzeeEndRoundLogic _endRoundLogic;
    private readonly YahtzeeGameContainer<D> _gameContainer;
    private readonly IToast _toast;
    public YahtzeeMove(ScoreContainer scoreContainer,
        IScoreLogic scoreLogic,
        YahtzeeVMData<D> model,
        IYahtzeeEndRoundLogic endRoundLogic,
        YahtzeeGameContainer<D> gameContainer,
        IToast toast
        )
    {
        _scoreContainer = scoreContainer;
        _scoreLogic = scoreLogic;
        _model = model;
        _endRoundLogic = endRoundLogic;
        _gameContainer = gameContainer;
        _toast = toast;
    }
    public async Task MakeMoveAsync(RowInfo row)
    {
        _scoreLogic.MarkScore(row);
        _gameContainer.SingleInfo!.Points = _scoreLogic.TotalScore;
        _gameContainer.SingleInfo.RowList = _scoreContainer.RowList.ToBasicList();
        _gameContainer.Command.UpdateAll();
        if (_gameContainer.Test.NoAnimations == false)
        {
            await _gameContainer.Delay.DelaySeconds(1);
        }
        _model.Cup!.UnholdDice();
        _model.Cup.HideDice();
        if (_gameContainer.PlayerList!.Any(x => x.MissNextTurn))
        {
            _toast.ShowInfoToast($"Everyone gets their turns skipped except for {_gameContainer.SingleInfo.NickName}.  Also, everyone will get a 0 for the category closest to the top because {_gameContainer.SingleInfo.NickName} got a Kismet even though it was already marked");
        }
        await _gameContainer.EndTurnAsync!.Invoke();
        await _endRoundLogic.StartNewRoundAsync();
    }
    public async Task MoveReceivedAsync(string data)
    {
        int id = int.Parse(data);
        RowInfo row = _scoreContainer.RowList[id];
        await MakeMoveAsync(row);
    }
}