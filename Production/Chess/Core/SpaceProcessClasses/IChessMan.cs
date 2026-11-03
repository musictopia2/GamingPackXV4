namespace Chess.Core.SpaceProcessClasses;
public interface IChessMan
{
    int Row { get; set; }
    int Column { get; set; }
    int Player { get; set; }
    bool IsReversed { get; set; }
    EnumPieceType PieceCategory { get; set; }
    BasicList<CheckersChessVector> GetValidMoves();
}