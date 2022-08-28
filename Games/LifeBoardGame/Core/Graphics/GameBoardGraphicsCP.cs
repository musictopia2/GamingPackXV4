namespace LifeBoardGame.Core.Graphics;
[SingletonGame]
[AutoReset]
public class GameBoardGraphicsCP
{
    private readonly IBoardProcesses _options;
    private readonly LifeBoardGameGameContainer _gameContainer;
    private readonly ISpacePosition _spacePos;
    public static int GoingTo { get; set; } //was going to put into container but it fits here more as static one.
    public static SizeF OriginalSize => new(800, 800);
    public TempInfo TempData;
    public GameBoardGraphicsCP(IBoardProcesses options,
        LifeBoardGameGameContainer gameContainer,
        ISpacePosition spacePos)
    {
        _options = options;
        _gameContainer = gameContainer;
        _spacePos = spacePos;
        _gameContainer.ExtraSpace = new();
        TempData = Resources.LifeBoardPositions.GetResource<TempInfo>();
        CreateSpaces();
    }
    private static RectangleF GetClickableRectangle(PositionInfo thisPos, bool isEnding)
    {
        var firstSize = new SizeF(10, 10);
        SizeF clickSize;
        if (isEnding == false)
        {
            clickSize = new SizeF(80, 80);
        }
        else
        {
            clickSize = new SizeF(140, 60);
        }
        var tempPoint = new PointF(thisPos.SpacePoint.X, thisPos.SpacePoint.Y);
        var thisPoint = new PointF(tempPoint.X - firstSize.Width, tempPoint.Y - firstSize.Height);
        return new RectangleF(thisPoint.X, thisPoint.Y, clickSize.Width, clickSize.Height);
    }
    private void CreateSpaces()
    {
        _gameContainer.CountrySpace = new();
        _gameContainer.CountrySpace.Area = GetCountrySideRect();
        _gameContainer.MillionSpace = new();
        _gameContainer.MillionSpace.Area = GetMillionRect();
    }
    private RectangleF GetCareerRectangle()
    {
        var thisPos = (from items in TempData.PositionList
                       where items.SpaceNumber == 201
                       select items).Single();
        return GetClickableRectangle(thisPos, false);
    }
    private RectangleF GetCollegeRectangle()
    {
        var thisPos = (from items in TempData.PositionList
                       where items.SpaceNumber == 202
                       select items).Single();
        return GetClickableRectangle(thisPos, false);
    }
    private RectangleF GetCountrySideRect()
    {
        var thisPos = (from items in TempData.PositionList
                       where items.SpaceNumber == 203
                       select items).Single();
        return GetClickableRectangle(thisPos, true);
    }
    private RectangleF GetMillionRect()
    {
        var thisPos = (from items in TempData.PositionList
                       where items.SpaceNumber == 204
                       select items).Single();
        return GetClickableRectangle(thisPos, true);
    }
    public BasicList<EndPositionInfo> CountrysideAcresOptions()
    {
        if (_gameContainer.CurrentView != EnumViewCategory.EndGame)
        {
            return new();
        }
        pp.ClearArea(_gameContainer.CountrySpace!);
        _gameContainer.CountrySpace!.PieceList.Clear();
        RectangleF piece;
        SizeF size;
        PointF point;
        var tempList = (from items in _gameContainer.PlayerList
                        where items.LastMove == EnumFinal.CountrySideAcres
                        select items).ToBasicList();
        BasicList<EndPositionInfo> output = new();
        foreach (var thisPlayer in tempList)
        {
            size = new SizeF(EndSize.Width, _gameContainer.CountrySpace.Area.Height);

            point = pp.GetPosition(_gameContainer.CountrySpace, size);
            piece = new RectangleF(point, size);
            EndPositionInfo end = new();
            end.Bounds = piece;
            end.Player = thisPlayer;
            output.Add(end);
            _gameContainer.CountrySpace.PieceList.Add(piece);
            pp.AddPieceToArea(_gameContainer.CountrySpace, piece);
        }
        return output;
    }
    public BasicList<EndPositionInfo> MillionaireEstatesOptions()
    {
        if (_gameContainer.CurrentView != EnumViewCategory.EndGame)
        {
            return new();
        }
        RectangleF piece;
        SizeF size;
        PointF point;
        BasicList<EndPositionInfo> output = new();
        pp.ClearArea(_gameContainer.MillionSpace!);
        _gameContainer.MillionSpace!.PieceList.Clear();
        var tempList = (from xx in _gameContainer.PlayerList
                        where xx.LastMove == EnumFinal.MillionaireEstates
                        select xx).ToBasicList();
        foreach (var thisPlayer in tempList)
        {
            size = new SizeF(EndSize.Width, _gameContainer.MillionSpace.Area.Height);
            point = pp.GetPosition(_gameContainer.MillionSpace, size);
            piece = new RectangleF(point, size);
            _gameContainer.MillionSpace.PieceList.Add(piece);
            pp.AddPieceToArea(_gameContainer.MillionSpace, piece);
            EndPositionInfo end = new();
            end.Bounds = piece;
            end.Player = thisPlayer;
            output.Add(end);
        }
        return output;
    }
    private static SizeF EndSize => new(34, 62);
    public void EndGameOptions()
    {
        if (_gameContainer.CurrentView != EnumViewCategory.EndGame)
        {
            throw new CustomBasicException("Needs to already be end game to use these options");
        }
        pp.ClearArea(_gameContainer.CountrySpace!);
        pp.ClearArea(_gameContainer.MillionSpace!);
        _gameContainer.CountrySpace!.PieceList.Clear();
        _gameContainer.MillionSpace!.PieceList.Clear();
        RectangleF piece;
        SizeF size;
        PointF point;
        SizeF main = new(34, 62);
        var tempList = (from items in _gameContainer.PlayerList
                        where items.LastMove == EnumFinal.CountrySideAcres
                        select items).ToBasicList();
        foreach (var thisPlayer in tempList)
        {
            size = new SizeF(main.Width, _gameContainer.CountrySpace.Area.Height);

            point = pp.GetPosition(_gameContainer.CountrySpace, size);
            piece = new RectangleF(point, size);
            _gameContainer.CountrySpace.PieceList.Add(piece);
            pp.AddPieceToArea(_gameContainer.CountrySpace, piece);

        }
        tempList = (from Items in _gameContainer.PlayerList
                    where Items.LastMove == EnumFinal.MillionaireEstates
                    select Items).ToBasicList();
        foreach (var thisPlayer in tempList)
        {

            size = new SizeF(main.Width, _gameContainer.MillionSpace.Area.Height);
            point = pp.GetPosition(_gameContainer.MillionSpace, size);
            piece = new RectangleF(point, size);
            _gameContainer.MillionSpace.PieceList.Add(piece);
            pp.AddPieceToArea(_gameContainer.MillionSpace, piece);
        }
    }
    public BasicList<ButtonInfo> GetMainActions()
    {
        if (_gameContainer.SingleInfo!.PlayerCategory != EnumPlayerCategory.Self)
        {
            return new();
        }
        RectangleF rect = GetExtraRectangle();
        _gameContainer.ExtraSpace!.Area = rect;
        _gameContainer.ExtraSpace.PieceList.Clear();
        ButtonInfo button;
        SizeF size = new(150, 50);
        PointF point;
        BasicList<ButtonInfo> output = new();
        if (_options.CanPurchaseCarInsurance)
        {
            button = new();
            button.Display = "Insure Car";
            button.Action = _options.PurchaseCarInsuranceAsync;
            button.Size = size;
            point = pp.GetPosition(_gameContainer.ExtraSpace!, size);
            button.Location = point;
            _gameContainer.ExtraSpace.PieceList.Add(new RectangleF(point, size));
            pp.AddPieceToArea(_gameContainer.ExtraSpace, new RectangleF(point, size));
            output.Add(button);
        }
        if (_options.CanPurchaseHouseInsurance)
        {
            button = new();
            button.Display = "Insure House";
            button.Action = _options.PurchaseHouseInsuranceAsync;
            button.Size = size;
            point = pp.GetPosition(_gameContainer.ExtraSpace, size);
            button.Location = point;
            _gameContainer.ExtraSpace.PieceList.Add(new RectangleF(point, size));
            pp.AddPieceToArea(_gameContainer.ExtraSpace, new RectangleF(point, size));
            output.Add(button);
        }
        if (_options.CanPurchaseStock)
        {
            button = new();
            button.Display = "Buy Stock";
            button.Action = _options.PurchaseStockAsync;
            button.Size = size;
            point = pp.GetPosition(_gameContainer.ExtraSpace, size);
            button.Location = point;
            _gameContainer.ExtraSpace.PieceList.Add(new RectangleF(point, size));
            pp.AddPieceToArea(_gameContainer.ExtraSpace, new RectangleF(point, size));
            output.Add(button);
        }
        if (_options.CanSellHouse)
        {
            button = new();
            button.Display = "Sell House";
            button.Action = _options.SellHouseAsync;
            button.Size = size;
            point = pp.GetPosition(_gameContainer.ExtraSpace, size);
            button.Location = point;
            _gameContainer.ExtraSpace.PieceList.Add(new RectangleF(point, size));
            pp.AddPieceToArea(_gameContainer.ExtraSpace, new RectangleF(point, size));
            output.Add(button);
        }
        if (_options.CanAttendNightSchool)
        {
            button = new();
            button.Display = "Night School";
            button.Action = _options.AttendNightSchoolAsync;
            button.Size = size;
            point = pp.GetPosition(_gameContainer.ExtraSpace, size);
            button.Location = point;
            _gameContainer.ExtraSpace.PieceList.Add(new RectangleF(point, size));
            pp.AddPieceToArea(_gameContainer.ExtraSpace, new RectangleF(point, size));
            output.Add(button);
        }
        if (_options.CanTrade4Tiles)
        {
            button = new();
            button.Display = "Trade 4 Tiles";
            button.Action = _options.Trade4TilesAsync;
            button.Size = size;
            point = pp.GetPosition(_gameContainer.ExtraSpace, size);
            button.Location = point;
            _gameContainer.ExtraSpace.PieceList.Add(new RectangleF(point, size));
            pp.AddPieceToArea(_gameContainer.ExtraSpace, new RectangleF(point, size));
            output.Add(button);
        }
        if (_gameContainer.GameStatus == EnumWhatStatus.NeedToChooseSpace)
        {
            button = new();
            button.Display = "Submit Space";
            button.Action = _options.HumanChoseSpaceAsync;
            button.Size = size;
            point = pp.GetPosition(_gameContainer.ExtraSpace, size);
            button.Location = point;
            _gameContainer.ExtraSpace.PieceList.Add(new RectangleF(point, size));
            pp.AddPieceToArea(_gameContainer.ExtraSpace, new RectangleF(point, size));
            output.Add(button);
        }
        if (_options.CanEndTurn)
        {
            button = new();
            button.Display = "End Turn";
            button.Action = _gameContainer.UIEndTurnAsync;
            button.Size = size;
            point = pp.GetPosition(_gameContainer.ExtraSpace, size);
            button.Location = point;
            _gameContainer.ExtraSpace.PieceList.Add(new RectangleF(point, size));
            pp.AddPieceToArea(_gameContainer.ExtraSpace, new RectangleF(point, size));
            output.Add(button);
        }
        return output;
    }
    public BasicList<ChooseSpaceInfo> GetSpaceChoices(PointF currentPoint)
    {
        if (_gameContainer.GameStatus != EnumWhatStatus.NeedToChooseSpace)
        {
            return new();
        }
        var firstPos = _spacePos.FirstPossiblePosition;
        var secondPos = _spacePos.SecondPossiblePosition;
        BasicList<ChooseSpaceInfo> output = new();
        ChooseSpaceInfo space = new();
        if (firstPos == _gameContainer.CurrentSelected)
        {
            space.Color = cc.Aqua;
        }
        else
        {
            space.Color = cc.Black;
        }
        var thisPos = (from xx in TempData.PositionList
                       where xx.PointView == currentPoint && xx.SpaceNumber == firstPos
                       select xx).Single();
        space.Bounds = GetClickableRectangle(thisPos, false);
        space.Space = firstPos;
        output.Add(space);
        space = new ChooseSpaceInfo();
        if (secondPos == _gameContainer.CurrentSelected)
        {
            space.Color = cc.Aqua;
        }
        else
        {
            space.Color = cc.Black;
        }
        thisPos = (from xx in TempData.PositionList
                   where xx.PointView == currentPoint && xx.SpaceNumber == secondPos
                   select xx).Single();
        space.Bounds = GetClickableRectangle(thisPos, false);
        space.Space = secondPos;
        output.Add(space);
        return output;
    }
    private RectangleF GetExtraRectangle()
    {
        int index;
        index = 300 + (int)_gameContainer.CurrentView;
        var thisPos = (from items in TempData.PositionList
                       where items.SpaceNumber == index
                       select items).Single();
        var firstPoint = new PointF(thisPos.SpacePoint.X, thisPos.SpacePoint.Y); // i think
        var thisSize = new SizeF(400, 300); // i think
        return new RectangleF(firstPoint.X, firstPoint.Y, thisSize.Width, thisSize.Height);
    }
    public BasicList<RectangleF> GoingToProcesses(PointF currentPoint)
    {
        if (GoingTo == 0)
        {
            return new();
        }
        var thisPos = (from Items in TempData.PositionList
                       where Items.PointView == currentPoint && Items.SpaceNumber == GoingTo
                       select Items).SingleOrDefault();
        if (thisPos == null)
        {
            return new();
        }
        return new() { GetClickableRectangle(thisPos, false) };
    }
    public BasicList<StartClickInfo> GetFirstOptions()
    {
        if (_gameContainer.GameStatus != EnumWhatStatus.NeedChooseFirstOption)
        {
            return new();
        }
        var firstRect = GetCareerRectangle();
        var secondRect = GetCollegeRectangle();
        return new()
        {
            new StartClickInfo(firstRect, EnumStart.Career),
            new StartClickInfo(secondRect, EnumStart.College)
        };
    }
    public BasicList<RetirementClickInfo> GetRetirementOptions()
    {
        if (_gameContainer.GameStatus != EnumWhatStatus.NeedChooseRetirement)
        {
            return new();
        }
        var firstRect = GetCountrySideRect();
        var secondRect = GetMillionRect();
        return new()
        {
            new RetirementClickInfo(firstRect, EnumFinal.CountrySideAcres),
            new RetirementClickInfo(secondRect, EnumFinal.MillionaireEstates)
        };
    }
}