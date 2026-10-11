namespace Backgammon.Core.Logic;
[SingletonGame]
[AutoReset]
public class GameBoardProcesses(BackgammonGameContainer gameContainer, GameBoardGraphicsCP graphicsBoard, BackgammonVMData model)
{
    #region "Reverse Processes"
    internal static int GetReversedID(int oldID)
    {
        if (oldID == 0)
        {
            return 27;
        }
        if (oldID == 25)
        {
            return 26;
        }
        if (oldID == 26)
        {
            return 25;
        }
        if (oldID == 27)
        {
            return 0;
        }
        return 25 - oldID;
    }
    private bool NeedsReversed(int playerConsidered)
    {
        var tempPlayer = gameContainer.PlayerList![playerConsidered];
        if (tempPlayer.PlayerCategory == EnumPlayerCategory.Computer)
        {
            return true;
        }
        if (gameContainer.BasicData!.MultiPlayer)
        {
            return tempPlayer.PlayerCategory == EnumPlayerCategory.OtherHuman;
        }
        if (gameContainer.PlayerList.Any(items => items.PlayerCategory == EnumPlayerCategory.Computer))
        {
            return false;
        }
        return gameContainer.WhoTurn != playerConsidered;
    }
    private TriangleClass GetTriangle(int player, int yourID, bool autoPopulate = true)
    {
        bool rets = NeedsReversed(player);
        int newID;
        if (rets == true)
        {
            newID = GetReversedID(yourID);
        }
        else
        {
            newID = yourID;
        }
        var thisTriangle = gameContainer.TriangleList[newID];
        if (autoPopulate)
        {
            thisTriangle.PlayerOwns = player;
        }
        return thisTriangle;
    }
    #endregion
    private void PopulateDiceValues()
    {
        var thisList = model.Cup!.DiceList;
        if (thisList.Count != 2)
        {
            throw new CustomBasicException("There should be just 2 dice values");
        }
        gameContainer.FirstDiceValue = thisList.First().Value;
        gameContainer.SecondDiceValue = thisList.Last().Value;
    }
    private void ClearTriangles()
    {
        foreach (var thisTriangle in gameContainer.TriangleList.Values)
        {
            thisTriangle.NumberOfTiles = 0;
            thisTriangle.PlayerOwns = 0;
            thisTriangle.Locations.Clear();
        }
    }
    private void PopulateTriangles()
    {
        TriangleClass thisTriangle;
        gameContainer.PlayerList!.ForEach(thisPlayer =>
        {
            if (thisPlayer.CurrentTurnData!.PiecesAtHome > 0)
            {
                thisTriangle = GetTriangle(thisPlayer.Id, 25);
                thisTriangle.NumberOfTiles = thisPlayer.CurrentTurnData.PiecesAtHome;
            }
            if (thisPlayer.CurrentTurnData.PiecesAtStart > 0)
            {
                thisTriangle = GetTriangle(thisPlayer.Id, 0);
                thisTriangle.NumberOfTiles = thisPlayer.CurrentTurnData.PiecesAtStart;
            }
            var thisList = thisPlayer.CurrentTurnData.PiecesOnBoard!.GroupBy(items => items).ToBasicList();
            thisList.ForEach(thisIndex =>
            {
                var ourIndex = thisIndex.Key;
                var count = thisIndex.Count();
                thisTriangle = GetTriangle(thisPlayer.Id, ourIndex);
                thisTriangle.NumberOfTiles = count;
            });
        });
    }
    private static int SpaceNumberForRoll(BasicList<int> thisCol, int whatNum, bool willReverse)
    {
        int thisNum;
        if (willReverse)
        {
            thisNum = GetReversedID(whatNum);
        }
        else
        {
            thisNum = whatNum;
        }
        if (thisCol.Any(items => items == thisNum))
        {
            return thisNum;
        }
        return 0;
    }
    private static int SpaceAboveRoll(BasicList<int> thisCol, int whatNum, bool willReverse)
    {
        int thisNum;
        if (willReverse)
        {
            thisNum = GetReversedID(whatNum);
        }
        else
        {
            thisNum = whatNum;
        }
        int finalValue = 0;
        int diffs = 50;
        int oldDiffs = 0;
        thisCol.ForEach(thisIndex =>
        {
            if (thisIndex > thisNum && willReverse == false)
            {
                oldDiffs = thisIndex - thisNum;
            }
            else if (thisIndex < thisNum && willReverse)
            {
                oldDiffs = thisNum - thisIndex;
            }
            if (oldDiffs < diffs)
            {
                diffs = oldDiffs;
                finalValue = thisIndex;
            }
        });
        return finalValue;
    }
    private static int OneToStack(int whatNum, BasicList<int> thisCol, bool willReverse)
    {
        var thisNum = SpaceNumberForRoll(thisCol, whatNum, willReverse);
        if (thisNum > 0)
        {
            return thisNum;
        }
        return SpaceAboveRoll(thisCol, whatNum, willReverse);
    }
    private EnumStatusType CalculateStatus(int index)
    {
        var thisSpace = gameContainer.TriangleList[index];
        if (thisSpace.PlayerOwns == 0)
        {
            return EnumStatusType.CompletelyOpen;
        }
        if (thisSpace.NumberOfTiles == 1)
        {
            if (thisSpace.PlayerOwns == gameContainer.WhoTurn)
            {
                return EnumStatusType.PlayerHasOne;
            }
            else
            {
                return EnumStatusType.KnockOtherPlayer;
            }
        }
        if (thisSpace.PlayerOwns == gameContainer.WhoTurn)
        {
            return EnumStatusType.PlayerOwns;
        }
        return EnumStatusType.Closed;
    }
    private static int NumberToMove(int whatNum, bool isReversed)
    {
        if (isReversed == false)
        {
            return whatNum;
        }
        return whatNum * -1;
    }
    private BasicList<int> ListSpacesFrom() => gameContainer.TriangleList.Where(items => items.Value.PlayerOwns == gameContainer.WhoTurn && items.Key > 0 && items.Key < 25).Select(items => items.Key).OrderBy(items => items).ToBasicList();
    private bool CanStackUp(bool isReversed)
    {
        var thisList = ListSpacesFrom();
        if (isReversed == false)
        {
            return thisList.First() >= 19;
        }
        return thisList.Last() <= 6;
    }
    private PointF CalculateDiffs(int spaceNumber)
    {
        if (spaceNumber == 0)
        {
            return new PointF(30, 0);
        }
        if (spaceNumber == 27)
        {
            return new PointF(-30, 0);
        }
        if (spaceNumber == 25 || spaceNumber == 26)
        {
            return new PointF(0, -11);
        }
        var thisTriangle = gameContainer.TriangleList[spaceNumber];
        int values;
        if (thisTriangle.NumberOfTiles > 5)
        {
            values = 15;
        }
        else
        {
            values = 28;
        }
        if (spaceNumber > 0 & spaceNumber < 13)
        {
            return new PointF(0, values);
        }
        else
        {
            return new PointF(0, values * -1);
        }
    }
    private static PointF CalculateFirstLocation(int spaceNumber, RectangleF tempRect)
    {
        var diffRight = new PointF(30, 0);
        var diffBottom = new PointF(0, 30);
        if (spaceNumber == 25 || spaceNumber == 26)
        {
            return new PointF(tempRect.Location.X, tempRect.Bottom - diffBottom.Y);
        }
        if (spaceNumber == 0)
        {
            return new PointF(tempRect.Location.X + 3, tempRect.Location.Y);
        }
        if (spaceNumber == 27)
        {
            return new PointF(tempRect.Right - diffRight.X, tempRect.Location.Y);
        }
        if (spaceNumber > 0 && spaceNumber < 13)
        {
            return new PointF(tempRect.Location.X + 3, tempRect.Location.Y);
        }
        else if (spaceNumber > 12 && spaceNumber < 25)
        {
            return new PointF(tempRect.Location.X + 3, tempRect.Bottom - diffBottom.Y);
        }
        else
        {
            throw new CustomBasicException("Cannot find the space");
        }
    }
    private PointF CalculateNewLocation(int spaceNumber)
    {
        var tempPoint = CalculateDiffs(spaceNumber);
        var diffx = tempPoint.X;
        var diffy = tempPoint.Y;
        var thisTriangle = gameContainer.TriangleList[spaceNumber];
        if (thisTriangle.NumberOfTiles == 0)
        {
            var tempRect = graphicsBoard.GetRectangleSpace(spaceNumber);
            return CalculateFirstLocation(spaceNumber, tempRect);
        }
        var thisLocation = thisTriangle.Locations.Last();
        tempPoint = thisLocation;
        tempPoint.X += diffx;
        tempPoint.Y += diffy;
        return tempPoint;
    }
    private BasicList<MoveInfo> MovesWithSpecificNumber(int value)
    {
        bool willReverse = gameContainer.SingleInfo!.PlayerCategory != EnumPlayerCategory.Self;
        bool stacks;
        bool fromStart;
        if (gameContainer.SingleInfo.CurrentTurnData!.PiecesAtStart > 0)
        {
            stacks = false;
            fromStart = true;
        }
        else
        {
            fromStart = false;
            stacks = CanStackUp(willReverse);
        }
        if (fromStart == true && stacks == true)
        {
            throw new CustomBasicException("Can't stack if from start");
        }
        EnumStatusType thisResult;
        MoveInfo thisMove;
        BasicList<MoveInfo> thisList = [];
        if (fromStart)
        {
            if (willReverse == false)
            {
                thisResult = CalculateStatus(value);
            }
            else
            {
                thisResult = CalculateStatus(GetReversedID(value));
            }
            if (thisResult == EnumStatusType.Closed)
            {
                return []; //because no moves can be made
            }
            thisMove = new()
            {
                DiceNumber = value,
                Results = thisResult
            };
            if (willReverse == false)
            {
                thisMove.SpaceFrom = 0;
                thisMove.SpaceTo = value;
            }
            else
            {
                thisMove.SpaceFrom = 27;
                thisMove.SpaceTo = GetReversedID(value);
            }
            return [thisMove];
        }
        var fromList = ListSpacesFrom();
        if (fromList.Count == 0)
        {
            return []; //because there was none.
        }
        int nextStart = 0;
        if (stacks)
        {
            nextStart = OneToStack(value, fromList, willReverse);
        }
        var newNum = NumberToMove(value, willReverse);
        int endMove;
        fromList.ForEach(thisIndex =>
        {
            endMove = thisIndex + newNum;
            if (endMove <= 0)
            {
                endMove = 26;
            }
            if (endMove > 24)
            {
                if (stacks && nextStart == thisIndex)
                {
                    thisMove = new()
                    {
                        Results = EnumStatusType.Stackup,
                        SpaceFrom = thisIndex,
                        DiceNumber = value
                    };
                    if (willReverse)
                    {
                        thisMove.SpaceTo = 26;
                    }
                    else
                    {
                        thisMove.SpaceTo = 25;
                    }
                    thisList.Add(thisMove);
                }
            }
            else
            {
                thisResult = CalculateStatus(endMove);
                if (thisResult != EnumStatusType.Closed)
                {
                    thisMove = new()
                    {
                        Results = thisResult,
                        SpaceFrom = thisIndex,
                        DiceNumber = value,
                        SpaceTo = endMove
                    };
                    thisList.Add(thisMove);
                }
            }
        });
        return thisList;
    }
    private void RepositionPieces()
    {
        var offsetSize = new PointF(3, 3);
        foreach (var thisTriangle in gameContainer.TriangleList.Values)
        {
            thisTriangle.Locations.Clear();
            if (thisTriangle.NumberOfTiles > 0)
            {
                int index = gameContainer.TriangleList.GetKey(thisTriangle);
                var thisRect = graphicsBoard.GetRectangleSpace(index);
                var tempPoint = CalculateDiffs(index);
                var thisLocation = CalculateFirstLocation(index, thisRect);
                var diffx = tempPoint.X;
                var diffy = tempPoint.Y;
                if (index > 0 && index < 25)
                {
                    thisLocation.X += offsetSize.X;
                }
                thisTriangle.NumberOfTiles.Times(x =>
                {
                    thisTriangle.Locations.Add(thisLocation);
                    thisLocation = new PointF(thisLocation.X + diffx, thisLocation.Y + diffy);
                });
            }
        }
        gameContainer.RefreshPieces = true; //maybe here (?)
    }
    private async Task BackToStartAsync(int player, int index, int oldPosition)
    {
        var thisPlayer = gameContainer.PlayerList![player];
        thisPlayer.CurrentTurnData!.PiecesOnBoard!.RemoveSpecificItem(oldPosition);
        thisPlayer.CurrentTurnData.PiecesAtStart++;
        gameContainer.Animates!.LocationTo = CalculateNewLocation(index);
        RepositionPieces();
        int tempTurn = gameContainer.WhoTurn;
        gameContainer.WhoTurn = tempTurn;
        await gameContainer.Animates.DoAnimateAsync();
        gameContainer.WhoTurn = tempTurn;
        var thisTriangle = gameContainer.TriangleList[index];
        thisTriangle.PlayerOwns = player;
        thisTriangle.NumberOfTiles += thisTriangle.NumberOfTiles;
        PopulateTriangles();
    }
    private void PopulateMoves()
    {
        gameContainer.MoveList.Clear();
        ClearTriangles();
        PopulateTriangles();
        bool rets = gameContainer.HadDoubles();
        BasicList<int> thisList = [];
        if (rets == true)
        {
            thisList.Add(gameContainer.FirstDiceValue);
        }
        else
        {
            if (gameContainer.SaveRoot!.NumberUsed != gameContainer.FirstDiceValue)
            {
                thisList.Add(gameContainer.FirstDiceValue);
            }
            if (gameContainer.SaveRoot.NumberUsed != gameContainer.SecondDiceValue)
            {
                thisList.Add(gameContainer.SecondDiceValue);
            }
        }
        if (thisList.Count == 0)
        {
            throw new CustomBasicException("Should not populate moves because all used up.  If I can ignore, take this out");
        }
        thisList.ForEach(thisValue =>
        {
            var tempList = MovesWithSpecificNumber(thisValue);
            gameContainer.MoveList.AddRange(tempList);
        });
        RepositionPieces();
    }
    private void LoadTriangles()
    {
        gameContainer.TriangleList = [];
        for (int x = 0; x <= 27; x++)
        {
            var thisTriangle = new TriangleClass();
            gameContainer.TriangleList.Add(x, thisTriangle);
        }
    }
    public void ClearBoard()
    {
        gameContainer.PlayerList!.ForEach(thisPlayer =>
        {
            thisPlayer.CurrentTurnData = new()
            {
                PiecesOnBoard = [1, 1, 12, 12, 12, 12, 12, 17, 17, 17, 19, 19, 19, 19, 19]
            };
        });
        LoadTriangles();
        gameContainer.MoveList = [];
        ClearTriangles();
        PopulateTriangles();
        gameContainer.SaveRoot!.SpaceHighlighted = -1;
        gameContainer.SaveRoot.NumberUsed = 0;
        RepositionPieces();
    }
    public void Repaint()
    {
        gameContainer.RepaintBoard();
    }
    private bool CanEndGame() => gameContainer.SingleInfo!.CurrentTurnData!.PiecesAtHome == 15;
    public async Task UndoAllMovesAsync()
    {
        gameContainer.SaveRoot!.NumberUsed = 0;
        gameContainer.SaveRoot.SpaceHighlighted = -1;
        gameContainer.MoveList.Clear();
        gameContainer.SaveRoot.MovesMade = 0;
        gameContainer.SaveRoot.MadeAtLeastOneMove = false;
        model.LastStatus = "You decided to undo all moves";
        gameContainer.PlayerList!.ForEach(player =>
        {
            player.CurrentTurnData = player.StartTurnData!.Clone();
        });
        gameContainer.SaveRoot.GameStatus = EnumGameStatus.MakingMoves;
        PopulateMoves();
        RepositionPieces();
        Repaint();
        if (gameContainer.DiceVisibleProcesses == null)
        {
            throw new CustomBasicException("Nobody is handling dice visible.  Rethink");
        }
        gameContainer.DiceVisibleProcesses.Invoke();
        await gameContainer.ContinueTurnAsync!.Invoke();
    }
    public void ReloadSavedGame()
    {
        if (gameContainer is null)
        {
            throw new CustomBasicException("Must have game container at least though");
        }
        //throw new CustomBasicException("No autoresume for now because of issues with repainting.  If i decided to do it eventually, then will have to put into interfaces which will do the work required when the first paint works.");
        LoadTriangles();
        PopulateDiceValues();
        PopulateMoves();
        gameContainer.DiceVisibleProcesses?.Invoke();
        //_graphicsBoard.Saved = true; //could be iffy
        //_thisE.RepaintBoard();
        //await Task.Delay(1000); //problem is this time, needs the actual coordinates for positioning.
        RepositionPieces();
        //_graphicsBoard.Saved = false;
        //_alreadyPainted = false;
    }
    //private bool _alreadyPainted;
    //public void StartPaint()
    //{
    //    if (_alreadyPainted)
    //        return;
    //    _thisE.RepaintBoard();
    //    _alreadyPainted = true;
    //}
    public async Task StartNewTurnAsync()
    {
        gameContainer.SaveRoot!.NumberUsed = 0;
        gameContainer.MoveList.Clear();
        gameContainer.SaveRoot.MovesMade = 0;
        gameContainer.SaveRoot.MadeAtLeastOneMove = false;
        foreach (var player in gameContainer.PlayerList!)
        {
            player.StartTurnData = player.CurrentTurnData!.Clone();
        }
        PopulateDiceValues();
        PopulateTriangles();
        PopulateMoves();
        RepositionPieces();
        Repaint();
        if (gameContainer.MoveList.Count == 0)
        {
            model.LastStatus = "No Moves Available";
            if (gameContainer.Test!.NoAnimations == false)
            {
                await gameContainer.Delay!.DelaySeconds(.5);
            }
            await gameContainer.EndTurnAsync!.Invoke();
            return;
        }
        gameContainer.SaveRoot.GameStatus = EnumGameStatus.MakingMoves;
        gameContainer.SaveRoot.SpaceHighlighted = -1;
    }
    public bool IsValidMove(int space)
    {
        if (gameContainer.SaveRoot!.SpaceHighlighted == space)
        {
            return true; //can always highlight/unhighlight
        }
        if (gameContainer.SaveRoot.SpaceHighlighted == -1)
        {
            return gameContainer.MoveList.Any(items => items.SpaceFrom == space);
        }
        return gameContainer.MoveList.Any(items => items.SpaceFrom == gameContainer.SaveRoot.SpaceHighlighted && items.SpaceTo == space);
    }
    public async Task MakeMoveAsync(int space)
    {
        if (gameContainer.SaveRoot!.SpaceHighlighted == space)
        {
            gameContainer.SaveRoot.SpaceHighlighted = -1;
            Repaint();
            await gameContainer.ContinueTurnAsync!.Invoke();
            return;
        }
        if (gameContainer.SaveRoot.SpaceHighlighted == -1)
        {
            gameContainer.SaveRoot.SpaceHighlighted = space;
            Repaint();
            await gameContainer.ContinueTurnAsync!.Invoke();
            return;
        }
        gameContainer.MoveInProgress = true;
        var thisMove = gameContainer.MoveList.First(items => items.SpaceFrom == gameContainer.SaveRoot.SpaceHighlighted && items.SpaceTo == space);
        var thisTriangle = gameContainer.TriangleList[gameContainer.SaveRoot.SpaceHighlighted];
        thisTriangle.NumberOfTiles--;
        var thisLocation = thisTriangle.Locations.Last();
        if (thisTriangle.NumberOfTiles == 0)
        {
            thisTriangle.PlayerOwns = 0;
        }
        gameContainer.Animates!.LocationFrom = thisLocation;
        gameContainer.Animates.LocationTo = CalculateNewLocation(space);
        RepositionPieces();
        gameContainer.RefreshPieces = true;
        Repaint();
        await Task.Delay(10);
        await gameContainer.Animates.DoAnimateAsync();
        var newTriangle = gameContainer.TriangleList[space];
        if (newTriangle.NumberOfTiles == 1)
        {
            if (newTriangle.PlayerOwns != gameContainer.WhoTurn)
            {
                var newPlayer = newTriangle.PlayerOwns;
                newTriangle.PlayerOwns = gameContainer.WhoTurn;
                gameContainer.Animates.LocationFrom = newTriangle.Locations.Single();
                if (NeedsReversed(newPlayer))
                {
                    await BackToStartAsync(newPlayer, 27, GetReversedID(space));
                }
                else
                {
                    await BackToStartAsync(newPlayer, 0, space);
                }
            }
            else
            {
                newTriangle.NumberOfTiles++;
            }
        }
        else
        {
            newTriangle.PlayerOwns = gameContainer.WhoTurn;
            newTriangle.NumberOfTiles++;
        }
        bool privateReverse = NeedsReversed(gameContainer.WhoTurn);
        int newSpace;
        if (privateReverse)
        {
            newSpace = GetReversedID(space);
        }
        else
        {
            newSpace = space;
        }
        if (gameContainer.SaveRoot.SpaceHighlighted == 0 || gameContainer.SaveRoot.SpaceHighlighted == 27)
        {
            gameContainer.SingleInfo!.CurrentTurnData!.PiecesAtStart--;
        }
        else
        {
            if (privateReverse)
            {
                gameContainer.SaveRoot.SpaceHighlighted = GetReversedID(gameContainer.SaveRoot.SpaceHighlighted);
            }
            gameContainer.SingleInfo!.CurrentTurnData!.PiecesOnBoard!.RemoveSpecificItem(gameContainer.SaveRoot.SpaceHighlighted);
        }
        if (space == 25 || space == 26)
        {
            gameContainer.SingleInfo.CurrentTurnData.PiecesAtHome++;
        }
        else
        {
            gameContainer.SingleInfo.CurrentTurnData.PiecesOnBoard!.Add(newSpace);
        }
        gameContainer.MoveInProgress = false;
        RepositionPieces();
        gameContainer.SaveRoot.SpaceHighlighted = -1;
        Repaint();
        if (CanEndGame() || gameContainer.Test.ImmediatelyEndGame)
        {
            await gameContainer.ShowWinAsync!.Invoke();
            return;
        }
        gameContainer.SaveRoot.MovesMade++;
        gameContainer.SaveRoot.MadeAtLeastOneMove = true;
        if (gameContainer.HadDoubles() == false)
        {
            if (gameContainer.SaveRoot.NumberUsed > 0)
            {
                if (gameContainer.DiceVisibleProcesses == null)
                {
                    throw new CustomBasicException("Nobody is handling dice visible.  Rethink");
                }
                gameContainer.DiceVisibleProcesses.Invoke();
                gameContainer.SaveRoot.GameStatus = EnumGameStatus.EndingTurn;
                await gameContainer.ContinueTurnAsync!.Invoke();
                return;
            }
            gameContainer.SaveRoot.NumberUsed = thisMove.DiceNumber;
            if (gameContainer.DiceVisibleProcesses == null)
            {
                throw new CustomBasicException("Nobody is handling dice visible.  Rethink");
            }
            gameContainer.DiceVisibleProcesses.Invoke();
            PopulateMoves();
            if (gameContainer.MoveList.Count == 0)
            {
                gameContainer.SaveRoot.GameStatus = EnumGameStatus.EndingTurn;
                await gameContainer.ContinueTurnAsync!.Invoke();
                return;
            }
            gameContainer.SaveRoot.GameStatus = EnumGameStatus.MakingMoves;
            await gameContainer.ContinueTurnAsync!.Invoke();
            return; //i think old version could had this bug.
        }
        if (gameContainer.DiceVisibleProcesses == null)
        {
            throw new CustomBasicException("Nobody is handling dice visible.  Rethink");
        }
        gameContainer.DiceVisibleProcesses.Invoke();
        if (gameContainer.SaveRoot.MovesMade == 4)
        {
            gameContainer.SaveRoot.GameStatus = EnumGameStatus.EndingTurn;
            await gameContainer.ContinueTurnAsync!.Invoke(); //becase you made all 4 moves.
            return;
        }
        PopulateMoves();
        Repaint();
        if (gameContainer.MoveList.Count == 0)
        {
            gameContainer.SaveRoot.GameStatus = EnumGameStatus.EndingTurn;
            await gameContainer.ContinueTurnAsync!.Invoke();
            return;
        }
        gameContainer.SaveRoot.GameStatus = EnumGameStatus.MakingMoves;
        await gameContainer.ContinueTurnAsync!.Invoke();
    }
}
