namespace Battleship.Core.Logic;
[SingletonGame]
[AutoReset]
public class ShipControlCP
{
    private readonly BattleshipVMData _model;
    private readonly GameBoardCP _gameBoard1;
    private readonly IToast _toast;
    public ShipControlCP(BattleshipVMData model, GameBoardCP gameBoard1, IToast toast)
    {
        _model = model;
        _gameBoard1 = gameBoard1;
        _toast = toast;
        LoadBoard();
    }
    private void LoadBoard()
    {
        BasicList<string> tempList = new()
        {
            "Carrier",
            "Battleship",
            "Cruiser",
            "Submarine",
            "Destroyer"
        };
        ShipList = new Dictionary<int, ShipInfoCP>();
        var loopTo = tempList.Count;
        int x;
        for (x = 1; x <= loopTo; x++)
        {
            ShipInfoCP ship = new ();
            ship.ShipName = tempList[x - 1];
            ship.PieceList = new Dictionary<int, PieceInfoCP>();
            ship.Visible = true;
            ship.ShipCategory = (EnumShipList)x; // i think
            ShipList.Add(ShipList.Count + 1, ship);
        }
        x = 0;
        int y;
        int q = default;
        foreach (var tempShip in ShipList.Values)
        {
            x += 1;
            int z;
            if (x == 1)
            {
                z = 5;
            }
            else if (x == 2)
            {
                z = 4;
            }
            else if (x == 3)
            {
                z = 3;
            }
            else
            {
                z = 2;
            }
            var loopTo1 = z;
            for (y = 1; y <= loopTo1; y++)
            {
                q += 1;
                PieceInfoCP thisPiece = new();
                thisPiece.Index = q;
                tempShip.PieceList!.Add(tempShip.PieceList.Count + 1, thisPiece); //since i had to update the varible name, causing many problems.
            }
        }
    }
    public Dictionary<int, ShipInfoCP> ShipList { get; set; } = new Dictionary<int, ShipInfoCP>();
    public EnumShipList ShipSelected
    {
        get
        {
            return _model.ShipSelected;
        }
        set
        {
            _model.ShipSelected = value;
        }
    }
    public void ClearBoard()
    {
        ShipSelected = EnumShipList.None;
        foreach (var thisShip in ShipList.Values)
        {
            thisShip.Visible = true;
            foreach (var piece in thisShip.PieceList!.Values)
            {
                piece.DidHit = false;
                piece.Location = "";
            }
        }
    }
    public bool CanPlaceShip(Vector space, bool horizontal)
    {
        if (ShipSelected == EnumShipList.None)
        {
            throw new CustomBasicException("Must choose a ship in order to figure out whether the ship can be placed or not");
        }
        BattleshipCollection HumanPositionList = _gameBoard1.HumanList!; //this uses the gameboard as well.
        int maxRows;
        int maxCols;
        FieldInfoCP newField;
        maxCols = 10; // that needs to change as well
        maxRows = 10; // needs to change as well
        int x;
        int howMany;
        ShipInfoCP thisShip;
        thisShip = ShipList[(int)ShipSelected];
        howMany = thisShip.PieceList!.Count;
        if (horizontal == true)
        {
            if (space.Column + howMany - 1 > maxCols)
            {
                _toast.ShowUserErrorToast("Ship out of bounds");
                return false;
            }
            var loopTo = howMany;
            for (x = 1; x <= loopTo; x++)
            {
                newField = HumanPositionList[space.Row, space.Column + x - 1];
                if (newField.ShipNumber != 0)
                {
                    _toast.ShowUserErrorToast("Ship out of bounds");
                    return false;
                }
            }
        }
        else
        {
            if ((space.Row + howMany - 1) > maxRows)
            {
                _toast.ShowUserErrorToast("Ship out of bounds");
                return false;
            }
            var loopTo1 = howMany;
            for (x = 1; x <= loopTo1; x++)
            {
                newField = HumanPositionList[space.Row + x - 1, space.Column];
                if (newField.ShipNumber != 0)
                {
                    _toast.ShowUserErrorToast("Ship out of bounds");
                    return false;
                }
            }
        }
        return true;
    }
    public bool IsFinished()
    {
        return ShipList.Values.All(Items => Items.Visible == false);
    }
    public bool HasSelectedShip()
    {
        return ShipSelected != EnumShipList.None;
    }
    public bool HasLost()
    {
        foreach (var thisShip in ShipList.Values)
        {
            foreach (var thisPiece in thisShip.PieceList!.Values)
            {
                if (thisPiece.DidHit == false)
                {
                    return false;
                }
            }
        }
        return true;
    }
    public void PlaceShip(Vector space, bool horizontal)
    {
        if (ShipSelected == EnumShipList.None)
        {
            throw new CustomBasicException("Must have a ship selected in order to place the ship");
        }
        var thisShip = ShipList[(int)ShipSelected];
        thisShip.Visible = false;
        foreach (var thisPiece in thisShip.PieceList!.Values)
        {
            int nextShip = thisPiece.Index;
            _gameBoard1.PlaceShip(space, nextShip, out string Info);
            thisPiece.Location = Info;
            if (horizontal == true)
            {
                space.Column++;
            }
            else
            {
                space.Row++;
            }
        }
        ShipSelected = EnumShipList.None;
    }
    private PieceInfoCP FindPiece(int index)
    {
        foreach (var thisShip in ShipList.Values)
        {
            foreach (var thisPiece in thisShip.PieceList!.Values)
            {
                if (thisPiece.Index == index)
                {
                    return thisPiece;
                }
            }
        }
        throw new CustomBasicException("Cannot find the piece with " + index.ToString());
    }
    public bool HasHit(Vector space)
    {
        FieldInfoCP thisField;
        var humanList = _gameBoard1.HumanList;
        thisField = humanList![space];
        if (thisField.ShipNumber > 0)
        {
            PieceInfoCP thisPiece;
            thisPiece = FindPiece(thisField.ShipNumber);
            thisPiece.DidHit = true;
            return true;
        }
        return false;
    }
}