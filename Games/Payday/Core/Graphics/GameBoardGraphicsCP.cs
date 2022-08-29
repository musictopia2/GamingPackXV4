namespace Payday.Core.Graphics;
[SingletonGame]
[AutoReset]
public class GameBoardGraphicsCP
{
    public struct PositionInfo
    {
        public RectangleF Bounds { get; set; }
        public string Color { get; set; }
    }
    public static SizeF OriginalSize => new(500, 500);
    public PaydayGameContainer GameContainer;
    public static RectangleF GetBounds => new(new PointF(0, 0), OriginalSize);
    public RectangleF LotteryRectangle { get; private set; }
    public GameBoardGraphicsCP(PaydayGameContainer gameContainer)
    {
        GameContainer = gameContainer;
        CreateSpaces();
    }
    public Dictionary<string, SpaceCP> SpaceList { get; private set; } = new ();
    private void CreateSpaces()
    {
        SpaceList.Clear();
        int int_Row;
        int int_Col;
        RectangleF rect;
        SpaceCP obj_Space;
        int int_Day = 0;
        RectangleF bounds = GetBounds;
        for (int_Row = 0; int_Row <= 4; int_Row++)
        {
            for (int_Col = 0; int_Col <= 6; int_Col++)
            {
                obj_Space = new();
                rect = new RectangleF(bounds.Left + ((bounds.Width / 7) * int_Col), bounds.Top + ((bounds.Height / 5) * int_Row), bounds.Width / 7, bounds.Height / 5);
                rect = new RectangleF(rect.Left + (rect.Width / 20), rect.Top + (rect.Height / 20), (rect.Width * 9) / 10, (rect.Height * 9) / 10);
                obj_Space.Number = int_Day;
                obj_Space.Bounds = rect;
                if (int_Day == 0)
                {
                    SpaceList.Add("Start", obj_Space);
                }
                else
                {
                    SpaceList.Add(int_Day.ToString(), obj_Space);
                }
                int_Day += 1;
            }
        }
        rect = new RectangleF(bounds.Left + ((bounds.Width / 7) * 4), bounds.Top + ((bounds.Height / 5) * 4), (bounds.Width * 3) / 7, bounds.Height / 5); // *** Draw jackpot
        rect = new RectangleF(rect.Left + (rect.Width / 50), rect.Top + (rect.Height / 20), rect.Width - (rect.Width / 30), (rect.Height * 9) / 10);
        obj_Space = new();
        obj_Space.Bounds = rect;
        obj_Space.Number = 32;
        SpaceList.Add("Finish", obj_Space);
        LotteryRectangle = new RectangleF(rect.Left, rect.Top + (rect.Height / 2), rect.Width, rect.Height / 2);
    }
    public RectangleF GetClickableSpace() => SpaceList[GameContainer.SaveRoot.NumberHighlighted.ToString()].Bounds;
    public RectangleF SpaceRectangle(int day)
    {
        if (SpaceList.ContainsKey(day.ToString()))
        {
            return SpaceList[day.ToString()].Bounds;
        }
        else
        {
            return default;
        }
    }
    public RectangleF StartingRectangle
    {
        get
        {
            if ((SpaceList!).ContainsKey("Start"))
            {
                return SpaceList["Start"].Bounds;
            }
            else
            {
                return default;
            }
        }
    }
    public RectangleF FinishRectangle
    {
        get
        {
            if ((SpaceList!).ContainsKey("Finish"))
            {
                return SpaceList["Finish"].Bounds;
            }
            else
            {
                return default;
            }
        }
    }
    public static BasicList<PositionInfo> GetPositionList(GameSpace space)
    {
        if (space.ColorList.Count != space.PieceList.Count)
        {
            return new();
        }
        BasicList<PositionInfo> output = new();
        for (int i = 0; i < space.PieceList.Count; i++)
        {
            output.Add(new PositionInfo()
            {
                Bounds = space.PieceList[i],
                Color = space.ColorList[i]
            });
        }
        return output;
    }
}