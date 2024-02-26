namespace BasicGameFrameworkLibrary.Core.SpecializedGameTypes.YahtzeeStyleHelpers.Logic;
public class YahtzeeMove<D>(ScoreContainer scoreContainer,
    IScoreLogic scoreLogic,
    YahtzeeVMData<D> model,
    IYahtzeeEndRoundLogic endRoundLogic,
    YahtzeeGameContainer<D> gameContainer,
    IToast toast
        ) : IYahtzeeMove, IMoveNM
    where D : SimpleDice, new()
{
    public async Task MakeMoveAsync(RowInfo row)
    {
        scoreLogic.MarkScore(row);
        gameContainer.SingleInfo!.Points = scoreLogic.TotalScore;
        gameContainer.SingleInfo.RowList = scoreContainer.RowList.ToBasicList();
        gameContainer.Command.UpdateAll();
        if (gameContainer.Test.NoAnimations == false)
        {
            await gameContainer.Delay.DelaySeconds(1);
        }
        model.Cup!.UnholdDice();
        model.Cup.HideDice();
        if (gameContainer.PlayerList!.Any(x => x.MissNextTurn))
        {
            toast.ShowInfoToast($"Everyone gets their turns skipped except for {gameContainer.SingleInfo.NickName}.  Also, everyone will get a 0 for the category closest to the top because {gameContainer.SingleInfo.NickName} got a Kismet even though it was already marked");
        }
        await gameContainer.EndTurnAsync!.Invoke();
        await endRoundLogic.StartNewRoundAsync();
    }
    public async Task MoveReceivedAsync(string data)
    {
        int id = int.Parse(data);
        RowInfo row = scoreContainer.RowList[id];
        await MakeMoveAsync(row);
    }
}