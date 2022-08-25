namespace SolitaireBoardGame.Core.Data;
[SingletonGame]
public class SolitaireBoardGameSaveInfo : IMappable, ISaveInfo
{
    public Vector PreviousPiece { get; set; }
    public SolitaireBoardGameCollection SpaceList = new();
}