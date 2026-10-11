namespace Cribbage.Core.Logic;
[SingletonGame]
[AutoReset]
public class HiddenBoard(CribbageGameContainer gameContainer)
{
    public async Task PegAsync(int howMany)
    {
        if (gameContainer.WhoTurn == 0)
        {
            throw new CustomBasicException("The whoturn cannot be 0");
        }
        if (gameContainer.NextStepAsync == null)
        {
            throw new CustomBasicException("Nobody is handling the next step");
        }
        if (howMany == 0)
        {
            await gameContainer.NextStepAsync();
            return;
        }
        int oldPosition;
        bool useFirsts;
        if (gameContainer.SingleInfo!.FirstPosition == 0 && gameContainer.SingleInfo.SecondPosition == 0)
        {
            oldPosition = 0;
            useFirsts = true;
        }
        else if (gameContainer.SingleInfo.FirstPosition > gameContainer.SingleInfo.SecondPosition)
        {
            oldPosition = gameContainer.SingleInfo.FirstPosition;
            useFirsts = false;
        }
        else
        {
            oldPosition = gameContainer.SingleInfo.SecondPosition;
            useFirsts = true;
        }
        int newPosition = oldPosition + howMany;
        if (newPosition > 121)
        {
            int diffs = newPosition - 121;
            howMany -= diffs;
            newPosition = 121;
        }
        gameContainer.SingleInfo.ScoreRound += howMany;
        int oldTotal = gameContainer.SingleInfo.TotalScore;
        gameContainer.SingleInfo.TotalScore = newPosition;
        if (gameContainer.SingleInfo.TotalScore < oldTotal)
        {
            throw new CustomBasicException("Can't lower score.  Rethink");
        }
        if (newPosition > 90)
        {
            gameContainer.SingleInfo.IsSkunk = false;
        }
        if (useFirsts == true)
        {
            gameContainer.SingleInfo.FirstPosition = newPosition;
        }
        else
        {
            gameContainer.SingleInfo.SecondPosition = newPosition;
        }
        if (newPosition == 121 || gameContainer.Test.ImmediatelyEndGame)
        {
            await gameContainer.ShowWinAsync!.Invoke();
            return;
        }
        await gameContainer.NextStepAsync.Invoke();
    }
}