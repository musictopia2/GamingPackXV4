namespace SolitaireBoardGame.Core.Logic; 
[SingletonGame]
public class SolitaireGameEventHandler : ISolitaireBoardEvents
{
    private readonly IToast _toast;
    public SolitaireGameEventHandler(IToast toast)
    {
        _toast = toast;
    }
    async Task ISolitaireBoardEvents.PiecePlacedAsync(GameSpace space, SolitaireBoardGameMainGameClass game)
    {
        if (game.IsValidMove(space) == false)
        {
            _toast.ShowUserErrorToast("Illegal Move");
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