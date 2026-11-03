namespace Mastermind.Core.Logic;
public interface IFinishGuess
{
    Task FinishGuessAsync(int howManyCorrect, GameBoardViewModel board);
}