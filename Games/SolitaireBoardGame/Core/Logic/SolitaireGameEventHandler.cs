namespace SolitaireBoardGame.Core.Logic; 
[SingletonGame]
public class SolitaireGameEventHandler(IToast toast) : ISolitaireBoardEvents
{
    async Task ISolitaireBoardEvents.PiecePlacedAsync(GameSpace space, SolitaireBoardGameMainGameClass game)
    {
        if (game.IsValidMove(space) == false)
        {
            toast.ShowUserErrorToast("Illegal Move");
            await game.UnselectPieceAsync(space);
            return;
        }
        await game.MakeMoveAsync(space);
    }
    async Task ISolitaireBoardEvents.PieceSelectedAsync(GameSpace space, SolitaireBoardGameMainGameClass game)
    {
        if (space.Vector.Equals(game.PreviousPiece) == false)
        {
            await game.HightlightSpaceAsync(space);
            return;
        }
        game.SelectUnSelectSpace(space);
    }
}