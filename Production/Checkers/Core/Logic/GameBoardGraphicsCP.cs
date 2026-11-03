namespace Checkers.Core.Logic;
[SingletonGame]
[AutoReset]
public class GameBoardGraphicsCP(CheckersGameContainer container) : CheckersChessBaseBoard<EnumColorChoice, SpaceCP>
{
    protected override void AfterClearBoard()
    {
        container.SpaceList = PrivateSpaceList;
    }

    public override bool CanHighlight()
    {
        return container.SaveRoot!.SpaceHighlighted > 0;
    }
    public override EnumCheckerChessGame GetGame()
    {
        return EnumCheckerChessGame.Checkers;
    }
}