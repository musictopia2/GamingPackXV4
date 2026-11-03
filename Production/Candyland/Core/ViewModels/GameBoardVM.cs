namespace Candyland.Core.ViewModels;
[SingletonGame]
[AutoReset]
public class GameBoardVM(CandylandMainGameClass mainGame, CommandContainer command)
{
    public async Task CastleAsync()
    {
        command.IsExecuting = true;
        await mainGame.GameOverAsync();
    }
    public async Task MakeMoveAsync(int space)
    {
        command.IsExecuting = true;
        await mainGame!.MakeMoveAsync(space);
    }
    public bool IsCastleValid
    {

        get
        {
            if (mainGame.GameBoard1.IsValidMove(127, mainGame.SaveRoot.CurrentCard!.WhichCard, mainGame, mainGame.SaveRoot.CurrentCard.HowMany))
            {
                return true;
            }
            return false;
        }
    }
    public int GetValidSpaceMove
    {
        get
        {
            for (int x = 1; x <= 126; x++)
            {
                if (mainGame.GameBoard1.IsValidMove(x, mainGame.SaveRoot.CurrentCard!.WhichCard, mainGame, mainGame.SaveRoot.CurrentCard.HowMany))
                {
                    return x;
                }
            }
            return 0;
        }
    }
}