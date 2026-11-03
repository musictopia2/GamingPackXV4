namespace BattleshipLite.Core.Logic;
public class BattleshipCollection : IBoardCollection<ShipInfo>
{
    private readonly BoardCollection<ShipInfo> _privateBoard;
    public ShipInfo this[int row, int column]
    {
        get
        {
            return _privateBoard[row, column];
        }
    }
    public ShipInfo this[Vector thisV] //when you click, it has to be a vector.
    {
        get
        {
            return _privateBoard[thisV];
        }
    }
    public BattleshipCollection()
    {
        _privateBoard = new(5, 5); //because we know its 5 by 5.
    }
    public BattleshipCollection(IEnumerable<ShipInfo> previousList)
    {
        _privateBoard = new(previousList);
    }
    public IEnumerator<ShipInfo> GetEnumerator()
    {
        return _privateBoard.GetEnumerator();
    }
    IEnumerator IEnumerable.GetEnumerator()
    {
        return _privateBoard.GetEnumerator();
    }
    public int GetTotalColumns()
    {
        return _privateBoard.GetTotalColumns();
    }
    public int GetTotalRows()
    {
        return _privateBoard.GetTotalRows();
    }
}