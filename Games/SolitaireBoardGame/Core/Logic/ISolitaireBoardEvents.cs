namespace SolitaireBoardGame.Core.Logic;
public interface ISolitaireBoardEvents
{
    Task PieceSelectedAsync(GameSpace space, SolitaireBoardGameMainGameClass game);
    Task PiecePlacedAsync(GameSpace space, SolitaireBoardGameMainGameClass game);
}