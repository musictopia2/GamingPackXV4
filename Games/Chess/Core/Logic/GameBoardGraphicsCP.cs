namespace Chess.Core.Logic;
[SingletonGame]
[AutoReset]
public class GameBoardGraphicsCP(ChessGameContainer gameContainer) : CheckersChessBaseBoard<EnumColorChoice, SpaceCP>
{
    protected override void AfterClearBoard()
    {
        gameContainer.SpaceList = PrivateSpaceList;
    }
    public override bool CanHighlight()
    {
        if (gameContainer.SaveRoot!.SpaceHighlighted > 0)
        {
            return true;
        }
        return gameContainer.SaveRoot.PreviousMove.SpaceFrom > 0 && gameContainer.SaveRoot.PreviousMove.SpaceTo > 0;
    }
    public override EnumCheckerChessGame GetGame()
    {
        return EnumCheckerChessGame.Chess;
    }
}