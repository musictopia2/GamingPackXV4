namespace Payday.Core.Logic;
[SingletonGame]
[AutoReset]
public class GameBoardProcesses(PaydayGameContainer gameContainer, GameBoardGraphicsCP boardGraphicsCP)
{
    public EnumDay GetDayDetails(int day)
    {
        if (day > 30)
        {
            throw new CustomBasicException("Only goes upto 30");
        }
        return _dayList[day - 1];
    }
    public int NextBuyerSpace()
    {
        return FindNextSpace(EnumDay.Buyer);
    }
    private int FindNextSpace(EnumDay whichOne)
    {
        var currentDay = gameContainer.SingleInfo!.DayNumber;
        var tempList = _dayList.Skip(currentDay);
        int x = 0;
        foreach (var thisItem in tempList)
        {
            x += 1;
            if ((int)thisItem == (int)whichOne)
            {
                return x + currentDay;
            }
        }
        throw new Exception("Can't find the next space on " + whichOne.ToString());
    }
    public int NextDealSpace()
    {
        return FindNextSpace(EnumDay.Deal);
    }
    readonly BasicList<EnumDay> _dayList =
        [
            EnumDay.Mail,
            EnumDay.SweepStakes,
            EnumDay.Mail,
            EnumDay.Deal,
            EnumDay.Mail,
            EnumDay.Lottery,
            EnumDay.SkiWeekEnd,
            EnumDay.RadioContest,
            EnumDay.Buyer,
            EnumDay.HappyBirthday,
            EnumDay.Mail,
            EnumDay.Deal,
            EnumDay.Lottery,
            EnumDay.CharityConcert,
            EnumDay.Deal,
            EnumDay.Mail,
            EnumDay.Buyer,
            EnumDay.Food,
            EnumDay.Mail,
            EnumDay.Lottery,
            EnumDay.YardSale,
            EnumDay.Mail,
            EnumDay.Buyer,
            EnumDay.Mail,
            EnumDay.Deal,
            EnumDay.Buyer,
            EnumDay.Lottery,
            EnumDay.ShoppingSpree,
            EnumDay.Buyer,
            EnumDay.WalkForCharity
        ];

    public void MoveToLastSpace()
    {
        gameContainer.SingleInfo!.InGame = false;
        PositionPieces();
    }
    private void PositionPieces()
    {
        if (gameContainer.PrivateSpaceList.Count == 0)
        {
            LoadBoard();
        }
        gameContainer.PrivateSpaceList.ForEach(thisSpace =>
        {
            pp1.ClearArea(thisSpace);
        });
        RectangleF rectanglePiece;
        float spaceSize;
        PointF location;
        GameSpace tempSpace;
        foreach (var thisPlayer in gameContainer.PlayerList!)
        {
            if (thisPlayer.InGame == true)
            {
                tempSpace = gameContainer.PrivateSpaceList[thisPlayer.DayNumber];
                spaceSize = tempSpace.Area.Width / 3.2f;
            }
            else
            {
                tempSpace = gameContainer.PrivateSpaceList[32];
                spaceSize = tempSpace.Area.Width / 7;
            }
            location = pp1.GetPosition(tempSpace, new SizeF(spaceSize, spaceSize));
            rectanglePiece = new RectangleF(location, new SizeF(spaceSize, spaceSize));
            tempSpace.PieceList.Add(rectanglePiece);
            tempSpace.ColorList.Add(thisPlayer.Color.Color);
            pp1.AddPieceToArea(tempSpace, rectanglePiece);
        }
        gameContainer.Aggregator.RepaintBoard();
    }
    public void ResetBoard()
    {
        foreach (var thisPlayer in gameContainer.PlayerList!)
        {
            thisPlayer.MoneyHas = 3500;
            thisPlayer.CurrentMonth = 1; // start out with month 1
            thisPlayer.DayNumber = 0; // start with day 0 because has not started yet
            thisPlayer.Loans = 0;
            thisPlayer.Hand.Clear(); // clear out the cards
            thisPlayer.ChoseNumber = 0;
            thisPlayer.InGame = true; //they are now in game.
        }
        PositionPieces();
    }
    private void LoadBoard()
    {
        int x;
        for (x = 0; x <= 32; x++)
        {
            GameSpace thisSpace = new();
            if (x >= 1 && x <= 31)
            {
                thisSpace.Area = boardGraphicsCP!.SpaceRectangle(x);
                thisSpace.Index = x;
            }
            else if (x == 0)
            {
                thisSpace.Area = boardGraphicsCP!.StartingRectangle;
            }
            else if (x == 32)
            {
                thisSpace.Area = boardGraphicsCP!.FinishRectangle;
            }
            else
            {
                throw new CustomBasicException("No rectangle found");
            }
            if (thisSpace.Area.Width == 0 || thisSpace.Area.Height == 0)
            {
                throw new CustomBasicException("No rectangle.  This means needs to rethink drawing or when creating spaces.");
            }
            gameContainer.PrivateSpaceList.Add(thisSpace);
        }
    }
    internal void ReloadSavedState()
    {
        if (gameContainer.PrivateSpaceList.Count == 0)
        {
            LoadBoard();
        }
        PositionPieces();
    }
    public void NewTurn()
    {
        gameContainer.Aggregator.Publish(new NewTurnEventModel());
        gameContainer.Aggregator.RepaintBoard();
        if (gameContainer.PrivateSpaceList.Count == 0)
        {
            LoadBoard();
        }
    }
    public void ClearJackPot()
    {
        gameContainer.SaveRoot!.LotteryAmount = 0;
        gameContainer.Aggregator.RepaintBoard();
    }
    public void AddToJackPot(decimal Amount)
    {
        gameContainer.SaveRoot!.LotteryAmount += Amount;
        gameContainer.Aggregator.RepaintBoard();
    }
    public void HighlightDay(int day)
    {
        gameContainer.SaveRoot!.NumberHighlighted = day;
        gameContainer.Aggregator.RepaintBoard();
    }
    public void UnhighlightDay()
    {
        gameContainer.SaveRoot!.NumberHighlighted = 0;
        gameContainer.Aggregator.RepaintBoard();
    }
    public async Task AnimateMoveAsync(int newDay)
    {
        if (newDay > 31)
        {
            throw new CustomBasicException("Must be upto 31.  If 32 is needed; then rethinking is required");
        }
        if (gameContainer.SingleInfo!.CanSendMessage(gameContainer.BasicData!))
        {
            await gameContainer.Network!.SendMoveAsync(newDay);
        }
        int x;
        x = gameContainer.SingleInfo!.DayNumber;
        do
        {
            x += 1;
            if (x == 32 && newDay < 32)
            {
                x = 1;
            }
            gameContainer.SingleInfo.DayNumber = x;
            PositionPieces();
            if (gameContainer.Test!.NoAnimations == false)
            {
                await gameContainer.Delay.DelayMilli(200);
            }
            if ((newDay >= 32 && x == 32) || x == newDay)
            {
                break;
            }
        }
        while (true);
        UnhighlightDay();
        if (gameContainer.ResultsOfMoveAsync == null)
        {
            throw new CustomBasicException("Nobody is handling the resultsofmoveasync.  Rethink");
        }
        await gameContainer.ResultsOfMoveAsync.Invoke(newDay);
    }
}
