namespace Chess.Core.SpaceProcessClasses;
public class KingClass : IChessMan
{
    public int Row { get; set; }
    public int Column { get; set; }
    public int Player { get; set; }
    public bool IsReversed { get; set; }
    public EnumPieceType PieceCategory { get; set; }
    public BasicList<CheckersChessVector> GetValidMoves()
    {
        BasicList<CheckersChessVector> possibleList = new()
        {
            new CheckersChessVector(Row + 1, Column + 1),
            new CheckersChessVector(Row - 1, Column - 1),
            new CheckersChessVector(Row + 1, Column),
            new CheckersChessVector(Row - 1, Column),
            new CheckersChessVector(Row, Column + 1),
            new CheckersChessVector(Row, Column - 1),
            new CheckersChessVector(Row - 1, Column + 1),
            new CheckersChessVector(Row + 1, Column - 1)
        };
        BasicList<CheckersChessVector> finalList = new();
        SpaceCP? c;
        foreach (var thisPos in possibleList)
        {
            c = GameBoardGraphicsCP.GetSpace(thisPos.Row, thisPos.Column);
            if (c != null)
            {
                if (c.PlayerOwns != Player)
                {
                    // can't be yourself period.
                    finalList.Add(thisPos);
                }
            }
        }
        return finalList;
    }
}
