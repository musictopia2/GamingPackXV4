namespace Kismet.Core.Logic;
[SingletonGame]
public class KismetMissTurn(
    YahtzeeVMData<KismetDice> model,
    TestOptions thisTest,
    IAsyncDelayer delay,
    IScoreLogic scoreLogic,
    ScoreContainer scoreContainer,
    YahtzeeGameContainer<KismetDice> gameContainer
        ) : IMissTurnClass<YahtzeePlayerItem<KismetDice>>
{
    public async Task PlayerMissTurnAsync(YahtzeePlayerItem<KismetDice> player)
    {
        model.NormalTurn = player.NickName;
        scoreContainer.RowList = player.RowList;
        scoreLogic.ClearRecent();
        RowInfo thisItem = player.RowList.Where(x => x.RowSection == EnumRow.Regular
            && x.HasFilledIn() == false).Take(1).SingleOrDefault()!;
        if (thisItem != null)
        {
            scoreLogic.MarkScore(thisItem);
            player.Points = scoreLogic.TotalScore; //their total score can change because of getting bonus.
            if (thisTest.NoAnimations == false)
            {
                if (gameContainer.GetNewScoreAsync == null)
                {
                    throw new CustomBasicException("Nobody is handling the getting new scores.  Rethink");
                }
                await gameContainer.GetNewScoreAsync();
                await delay.DelayMilli(1200);
            }
        }
    }
}